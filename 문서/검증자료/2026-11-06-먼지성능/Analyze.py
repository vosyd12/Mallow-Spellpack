import csv
import hashlib
import json
import statistics as stats
from pathlib import Path

root = Path('Library/DustPerformance')
data = json.loads((root / 'results.json').read_text())
gc_control = json.loads((root / 'gc-control.json').read_text())
call_allocation_valid = gc_control.get('data', {}).get('result', {}).get('delta', 0) > 0
def percentile(values, p):
    values = sorted(values)
    index = (len(values) - 1) * p
    low = int(index)
    high = min(low + 1, len(values) - 1)
    return values[low] + (values[high] - values[low]) * (index - low)

rows = []
for case in data['results']:
    frames = [f['ms'] for f in case['frames']]
    allocated = [f['gcBytes'] for f in case['frames'] if f['gcBytes'] >= 0]
    rows.append({
        'rate': case['rate'], 'mode': case['mode'], 'trial': case['trial'],
        'valid': case['requests'] == case['returns'],
        'requests': case['requests'], 'returns': case['returns'],
        'created': case['created'], 'destroyed': case['destroyed'],
        'callCpuUsPerEffect': case['callMs'] * 1000 / case['requests'],
        'managedCallBytesPerEffect': case['managedCallBytes'] / case['requests'] if call_allocation_valid else None,
        'frameMeanMs': stats.mean(frames), 'frameMedianMs': stats.median(frames),
        'frameP95Ms': percentile(frames, .95), 'frameP99Ms': percentile(frames, .99),
        'frameMaxMs': max(frames), 'framesOver16_67Percent': sum(v > 16.67 for v in frames) * 100 / len(frames),
        'frameGcMeanBytes': stats.mean(allocated) if allocated else None,
        'frameGcMeanKiB': stats.mean(allocated) / 1024 if allocated else None,
        'gc0': case['gc0'], 'gc1': case['gc1'], 'gc2': case['gc2'],
        'prewarmMs': case['prewarmMs'], 'prewarmBytes': case['prewarmBytes'] if call_allocation_valid else None,
        'frames': len(frames),
    })

metrics = ['callCpuUsPerEffect', 'managedCallBytesPerEffect', 'created', 'destroyed', 'frameMeanMs', 'frameMedianMs', 'frameP95Ms', 'frameP99Ms', 'frameMaxMs', 'framesOver16_67Percent', 'prewarmMs', 'prewarmBytes', 'frameGcMeanBytes', 'frameGcMeanKiB']
summary = []
for rate in [70, 350, 700]:
    for mode in ['instantiate_destroy', 'pool']:
        group = [r for r in rows if r['rate'] == rate and r['mode'] == mode and r['valid']]
        result = {'rate': rate, 'mode': mode, 'trials': len(group), 'requestsPerTrial': group[0]['requests']}
        for metric in metrics:
            values = [r[metric] for r in group if r[metric] is not None]
            result[metric] = stats.median(values) if values else None
            result[metric + 'Range'] = [min(values), max(values)] if values else None
        result['gcCollections'] = [[r['gc0'], r['gc1'], r['gc2']] for r in group]
        summary.append(result)
(root / 'summary.json').write_text(json.dumps(summary, indent=2), encoding='utf-8')
with (root / 'trials.csv').open('w', newline='', encoding='utf-8-sig') as output:
    writer = csv.DictWriter(output, fieldnames=list(rows[0]))
    writer.writeheader()
    writer.writerows(rows)
fingerprints = {p.as_posix(): hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets/Scripts').rglob('*.cs')}
(root / 'source-sha256.json').write_text(json.dumps(fingerprints, indent=2), encoding='utf-8')
for r in summary:
    print(r['rate'], r['mode'], 'CPU us', round(r['callCpuUsPerEffect'], 3), 'frame GC KiB', round(r['frameGcMeanKiB'], 2), 'created', r['created'], 'frame p95', round(r['frameP95Ms'],3))
