import type { CalculatorFormValues } from '../types/tax'

export const createInitialValues = (): CalculatorFormValues => ({
  taxYear: '2026-27', ageCategory: 'below60', grossSalary: 0,
  salaryAllowancesExemptBothRegimes: 0, otherIncome: 0, housePropertyInterest: 0,
  oldRegimeDeductions: { section80C: 0, section80D: 0, otherDeductions: 0 }, newRegimeDeductions: 0,
})
