export type AgeCategory = 'below60' | '60to79' | '80plus'

export type TaxAmountInput = number | string

export type OldRegimeDeductionField = 'section80C' | 'section80D' | 'otherDeductions'

export type CalculatorFieldName =
  | 'taxYear'
  | 'ageCategory'
  | 'grossSalary'
  | 'salaryAllowancesExemptBothRegimes'
  | 'otherIncome'
  | 'housePropertyInterest'
  | 'newRegimeDeductions'
  | `oldRegimeDeductions.${OldRegimeDeductionField}`

export interface OldRegimeDeductionsInput {
  section80C: TaxAmountInput
  section80D: TaxAmountInput
  otherDeductions: TaxAmountInput
}

export interface CalculatorFormValues {
  taxYear: string
  ageCategory: AgeCategory
  grossSalary: TaxAmountInput
  salaryAllowancesExemptBothRegimes: TaxAmountInput
  otherIncome: TaxAmountInput
  housePropertyInterest: TaxAmountInput
  oldRegimeDeductions: OldRegimeDeductionsInput
  newRegimeDeductions: TaxAmountInput
}

export interface OldRegimeDeductionsRequest {
  section80C: number
  section80D: number
  otherDeductions: number
}

export interface CompareTaxRequest {
  taxYear: string
  ageCategory: AgeCategory
  grossSalary: number
  salaryAllowancesExemptBothRegimes: number
  otherIncome: number
  housePropertyInterest: number
  oldRegimeDeductions: OldRegimeDeductionsRequest
  newRegimeDeductions: number
}

export type ResultStatus = 'Success' | 'ValidationFailure' | 'Error'

export interface OperationResult<T> {
  resultStatus: ResultStatus
  data: T | null
  message: string
}

export interface TaxRegimeResult {
  grossTotalIncome: number
  standardDeduction: number
  totalDeductions: number
  taxableIncome: number
  slabTax: number
  rebate87A: number
  taxAfterRebate: number
  surcharge: number
  healthAndEducationCess: number
  finalTax: number
  warnings: string[]
}

export type RecommendedRegime = 'Old' | 'New' | 'Same'

export interface TaxComparisonData {
  taxYear: string
  recommendedRegime: RecommendedRegime
  savings: number
  oldRegime: TaxRegimeResult
  newRegime: TaxRegimeResult
}
