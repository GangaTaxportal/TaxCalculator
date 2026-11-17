using TaxCalculatorAPI.Models;
namespace TaxCalculatorAPI.Services;
public sealed class SlabCalculator
{
    public decimal Calculate(decimal income, IReadOnlyList<TaxSlab> slabs)
    {
        decimal tax = 0, lower = 0;
        foreach (var slab in slabs)
        {
            var upper = slab.UpTo == 0 ? income : Math.Min(income, slab.UpTo);
            if (upper > lower) tax += (upper - lower) * slab.Rate;
            if (slab.UpTo == 0 || income <= slab.UpTo) break;
            lower = slab.UpTo;
        }
        return tax;
    }
}
