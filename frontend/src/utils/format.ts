export const formatRupees = (value: number | string | null | undefined): string => new Intl.NumberFormat('en-IN', {
  style: 'currency', currency: 'INR', maximumFractionDigits: 0,
}).format(Number(value) || 0)

export const formatLabel = (value: string | null | undefined): string => String(value || '').replace(/([a-z])([A-Z])/g, '$1 $2').replace(/^./, (letter) => letter.toUpperCase())
