import type { CompareTaxRequest, OperationResult, ResultStatus, TaxComparisonData, TaxRegimeResult } from '../types/tax'

const BASE = '/api'

export class TaxApiError extends Error {
  readonly messages: string[]

  constructor(message: string, messages: string[] = [message]) {
    super(message)
    this.name = 'TaxApiError'
    this.messages = messages
  }
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null && !Array.isArray(value)
}

function isResultStatus(value: unknown): value is ResultStatus {
  return value === 'Success' || value === 'ValidationFailure' || value === 'Error'
}

function normalizeOperationResult(value: unknown): OperationResult<unknown> | null {
  if (!isRecord(value)) return null
  const resultStatus = value.resultStatus ?? value.ResultStatus
  const message = value.message ?? value.Message
  const hasData = 'data' in value || 'Data' in value
  if (!isResultStatus(resultStatus) || typeof message !== 'string' || !hasData) return null
  return {
    resultStatus,
    message,
    data: value.data ?? value.Data ?? null,
  }
}

function isNumber(value: unknown): value is number {
  return typeof value === 'number' && Number.isFinite(value)
}

function isTaxRegimeResult(value: unknown): value is TaxRegimeResult {
  if (!isRecord(value)) return false
  const numericFields: (keyof Omit<TaxRegimeResult, 'warnings'>)[] = [
    'grossTotalIncome', 'standardDeduction', 'totalDeductions', 'taxableIncome', 'slabTax',
    'rebate87A', 'taxAfterRebate', 'surcharge', 'healthAndEducationCess', 'finalTax',
  ]
  return numericFields.every((field) => isNumber(value[field]))
    && Array.isArray(value.warnings)
    && value.warnings.every((warning) => typeof warning === 'string')
}

function isTaxComparisonData(value: unknown): value is TaxComparisonData {
  if (!isRecord(value)) return false
  return typeof value.taxYear === 'string'
    && (value.recommendedRegime === 'Old' || value.recommendedRegime === 'New' || value.recommendedRegime === 'Same')
    && isNumber(value.savings)
    && isTaxRegimeResult(value.oldRegime)
    && isTaxRegimeResult(value.newRegime)
}

function getValidationMessages(payload: unknown): string[] {
  if (!isRecord(payload) || !isRecord(payload.errors)) return []
  return Object.values(payload.errors).flatMap((value) =>
    Array.isArray(value) ? value.filter((message): message is string => typeof message === 'string') : [],
  )
}

function getHttpErrorMessage(payload: unknown, responseText: string, status: number): string {
  if (isRecord(payload)) {
    for (const key of ['message', 'detail', 'title']) {
      const message = payload[key]
      if (typeof message === 'string' && message.length > 0) return message
    }
  }
  return responseText || `Request failed (${status}).`
}

export async function compareTax(request: CompareTaxRequest): Promise<TaxComparisonData> {
  const response = await fetch(`${BASE}/tax/compare`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
    body: JSON.stringify(request),
  })

  const responseText = await response.text()
  let payload: unknown = null
  try {
    payload = responseText ? JSON.parse(responseText) as unknown : null
  } catch {
    // Keep a null payload when the response body is not valid JSON.
  }

  if (!response.ok) {
    const messages = getValidationMessages(payload)
    const message = getHttpErrorMessage(payload, responseText, response.status)
    throw new TaxApiError(message, messages.length > 0 ? messages : [message])
  }

  // Retain support for the legacy response body while the backend rollout is in progress.
  if (isTaxComparisonData(payload)) return payload

  const operationResult = normalizeOperationResult(payload)
  if (!operationResult) {
    throw new TaxApiError('The tax API returned an invalid response envelope.')
  }

  switch (operationResult.resultStatus) {
    case 'Success':
      if (!isTaxComparisonData(operationResult.data)) {
        throw new TaxApiError(operationResult.message || 'The tax API returned a successful status without valid result data.')
      }
      return operationResult.data
    case 'ValidationFailure':
    case 'Error':
      throw new TaxApiError(operationResult.message || 'The tax calculation failed.')
    default:
      throw new TaxApiError('The tax API returned an unsupported result status.')
  }
}
