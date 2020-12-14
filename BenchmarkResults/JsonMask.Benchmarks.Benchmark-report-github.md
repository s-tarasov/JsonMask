``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.685 (2004/?/20H1)
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4300.0), X64 RyuJIT
  Job-NJYLUU : .NET Framework 4.8 (4.8.4300.0), X64 RyuJIT

Force=False  

```
|                        Method |           FileName |           Mean |         Error |        StdDev |         Median |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
|------------------------------ |------------------- |---------------:|--------------:|--------------:|---------------:|----------:|----------:|---------:|------------:|
|         **JsonMasker_MaskByPath** | **example-large.json** |   **9,410.172 μs** |    **19.8230 μs** |    **16.5531 μs** |   **9,404.369 μs** | **1140.6250** |  **390.6250** | **312.5000** |  **4196.37 KB** |
| JsonMasker_MaskByPropertyName | example-large.json |   7,783.417 μs |    50.4347 μs |    47.1767 μs |   7,798.199 μs |  898.4375 |  281.2500 | 257.8125 |  3634.21 KB |
|   RegEx_ReplaceByPropertyName | example-large.json |   5,964.174 μs |    66.1315 μs |    61.8595 μs |   5,954.827 μs |  757.8125 |  546.8750 | 312.5000 |  4135.62 KB |
|     JObject_ParseAndSerialize | example-large.json |  34,992.388 μs |   680.3192 μs | 1,038.9193 μs |  35,135.227 μs | 1800.0000 |  800.0000 | 266.6667 |  10983.5 KB |
|  JsonMasking_MaskFieldsByPath | example-large.json | 104,698.088 μs | 2,072.2149 μs | 4,279.4831 μs | 105,023.742 μs | 8500.0000 | 2166.6667 | 666.6667 | 34256.27 KB |
|         **JsonMasker_MaskByPath** | **example-small.json** |       **6.630 μs** |     **0.1125 μs** |     **0.0998 μs** |       **6.680 μs** |    **1.7548** |         **-** |        **-** |     **5.41 KB** |
| JsonMasker_MaskByPropertyName | example-small.json |       5.327 μs |     0.0202 μs |     0.0189 μs |       5.327 μs |    1.6327 |         - |        - |     5.03 KB |
|   RegEx_ReplaceByPropertyName | example-small.json |       4.695 μs |     0.0616 μs |     0.0577 μs |       4.684 μs |    1.2665 |         - |        - |      3.9 KB |
|     JObject_ParseAndSerialize | example-small.json |      14.067 μs |     0.0968 μs |     0.0905 μs |      14.075 μs |    4.2572 |         - |        - |    13.12 KB |
|  JsonMasking_MaskFieldsByPath | example-small.json |      55.029 μs |     0.4928 μs |     0.4115 μs |      55.143 μs |   12.2070 |         - |        - |    37.54 KB |
|         **JsonMasker_MaskByPath** |       **example.json** |      **79.488 μs** |     **1.5778 μs** |     **3.3281 μs** |      **78.036 μs** |   **15.2588** |         **-** |        **-** |    **47.21 KB** |
| JsonMasker_MaskByPropertyName |       example.json |      63.823 μs |     0.3086 μs |     0.2887 μs |      63.819 μs |   13.6719 |         - |        - |    42.25 KB |
|   RegEx_ReplaceByPropertyName |       example.json |      45.594 μs |     0.4556 μs |     0.4261 μs |      45.656 μs |   14.4653 |         - |        - |    44.65 KB |
|     JObject_ParseAndSerialize |       example.json |     163.942 μs |     1.8584 μs |     1.7383 μs |     164.646 μs |   39.0625 |         - |        - |   120.26 KB |
|  JsonMasking_MaskFieldsByPath |       example.json |     511.133 μs |     7.0319 μs |     6.5776 μs |     512.274 μs |  104.4922 |         - |        - |   324.05 KB |
