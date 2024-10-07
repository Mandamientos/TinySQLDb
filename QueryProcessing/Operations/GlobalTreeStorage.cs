using QueryProcessing.Operations;
using System.Collections.Generic;

public static class GlobalTreeStorage
{
    // Static list to hold multiple ArbolBinario objects
    public static List<ArbolBinario> Trees { get; } = new List<ArbolBinario>();
}
