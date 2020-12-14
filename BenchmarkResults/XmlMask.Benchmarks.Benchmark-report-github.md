``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.685 (2004/?/20H1)
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4300.0), X64 RyuJIT
  Job-RMAIOO : .NET Framework 4.8 (4.8.4300.0), X64 RyuJIT

Force=False  IterationCount=15  LaunchCount=2  
WarmupCount=10  

```
|                             Method |          FileName |         Mean |        Error |       StdDev |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
|----------------------------------- |------------------ |-------------:|-------------:|-------------:|----------:|----------:|---------:|------------:|
|        **XmlMasker_MaskByElementName** | **example-large.xml** | **14,242.27 μs** |   **197.811 μs** |   **277.304 μs** |  **234.3750** |  **234.3750** | **234.3750** |  **3502.21 KB** |
| XmlMasker_MaskByUnknownElementName | example-large.xml | 10,932.33 μs |   121.987 μs |   182.584 μs |  234.3750 |  234.3750 | 234.3750 |  3502.21 KB |
|              XmlMasker_MaskByXpath | example-large.xml | 36,157.21 μs |   921.508 μs | 1,350.735 μs |  800.0000 |  533.3333 | 333.3333 | 10837.55 KB |
|                      RegEx_Replace | example-large.xml | 12,378.63 μs |   277.197 μs |   406.312 μs |  890.6250 |  625.0000 | 312.5000 |  7869.25 KB |
|                               XSLT | example-large.xml | 61,444.09 μs | 1,355.657 μs | 1,944.243 μs | 2777.7778 | 1777.7778 | 888.8889 | 18586.68 KB |
|        XDocument_ParseAndSerialize | example-large.xml | 42,074.62 μs | 1,034.663 μs | 1,483.883 μs | 2384.6154 | 1000.0000 | 461.5385 | 12201.97 KB |
|        **XmlMasker_MaskByElementName** | **example-small.xml** |     **26.59 μs** |     **0.444 μs** |     **0.651 μs** |    **6.9580** |         **-** |        **-** |    **21.45 KB** |
| XmlMasker_MaskByUnknownElementName | example-small.xml |     20.28 μs |     0.374 μs |     0.548 μs |    6.9580 |    0.0305 |        - |    21.45 KB |
|              XmlMasker_MaskByXpath | example-small.xml |     56.36 μs |     1.224 μs |     1.794 μs |   14.2822 |         - |        - |    44.04 KB |
|                      RegEx_Replace | example-small.xml |     17.15 μs |     0.285 μs |     0.427 μs |    5.0354 |         - |        - |    15.57 KB |
|                               XSLT | example-small.xml |     99.49 μs |     0.822 μs |     1.230 μs |   20.2637 |    0.1221 |        - |    62.59 KB |
|        XDocument_ParseAndSerialize | example-small.xml |     55.01 μs |     0.585 μs |     0.875 μs |   16.7236 |         - |        - |    51.51 KB |
|        **XmlMasker_MaskByElementName** |       **example.xml** |    **196.65 μs** |     **3.036 μs** |     **4.450 μs** |   **20.2637** |         **-** |        **-** |    **63.61 KB** |
| XmlMasker_MaskByUnknownElementName |       example.xml |    133.62 μs |     3.180 μs |     4.760 μs |   20.2637 |    0.2441 |        - |    63.61 KB |
|              XmlMasker_MaskByXpath |       example.xml |    393.25 μs |     2.376 μs |     3.483 μs |   59.0820 |    0.4883 |        - |   183.54 KB |
|                      RegEx_Replace |       example.xml |    109.20 μs |     0.983 μs |     1.471 μs |   38.4521 |         - |        - |   118.76 KB |
|                               XSLT |       example.xml |    759.35 μs |     7.019 μs |    10.506 μs |   82.0313 |   20.5078 |        - |   298.17 KB |
|        XDocument_ParseAndSerialize |       example.xml |    391.76 μs |     4.903 μs |     6.873 μs |   62.0117 |    5.3711 |   0.4883 |   195.86 KB |
