import { formatRupees } from '../utils/format'
import type { TaxComparisonData, TaxRegimeResult } from '../types/tax'

const rows = [
  ['Gross Total Income', 'grossTotalIncome'], ['Standard Deduction', 'standardDeduction'], ['Total Deductions', 'totalDeductions'],
  ['Taxable Income', 'taxableIncome'], ['Slab Tax', 'slabTax'], ['Section 87A Rebate', 'rebate87A'],
  ['Tax After Rebate', 'taxAfterRebate'], ['Surcharge', 'surcharge'], ['Health and Education Cess', 'healthAndEducationCess'],
] as const satisfies ReadonlyArray<readonly [string, keyof TaxRegimeResult]>

interface RegimeCardProps {
  title: 'Old Regime' | 'New Regime'
  data: TaxRegimeResult
  recommended: boolean
}

function RegimeCard({ title, data, recommended }: RegimeCardProps) {
  return <article className={`regime-card ${recommended ? 'recommended' : ''}`}>
    <div className="regime-heading"><div><p className="regime-kicker">{title === 'Old Regime' ? 'DEDUCTIONS FIRST' : 'SIMPLER FILING'}</p><h3>{title}</h3></div>{recommended && <span className="recommended-badge"><span>✦</span> Recommended</span>}</div>
    <div className="result-rows">{rows.map(([label, key]) => <div className="result-row" key={key}><span>{label}</span><strong>{formatRupees(data?.[key])}</strong></div>)}</div>
    {data?.warnings?.length > 0 && <div className="warnings"><strong>Heads up</strong><ul>{data.warnings.map((warning, index) => <li key={`${warning}-${index}`}>{warning}</li>)}</ul></div>}
    <div className="final-tax"><div><span>Health and Education Cess included</span><strong>Final tax</strong></div><strong className="final-amount">{formatRupees(data?.finalTax)}</strong></div>
  </article>
}

interface ComparisonResultsProps {
  result: TaxComparisonData
}

export default function ComparisonResults({ result }: ComparisonResultsProps) {
  const recommendation = String(result.recommendedRegime || '').toLowerCase()
  const recommendedOld = recommendation === 'old'
  const recommendedNew = recommendation === 'new'
  const tied = recommendation === 'same'
  return <div className="comparison-results"><div className="results-title"><div><p className="eyebrow">YOUR TAX BREAKDOWN</p><h2>Regime comparison</h2><p>Tax year {result.taxYear}</p></div><div className="savings-pill"><span>{tied ? 'Same tax under both regimes' : 'Potential tax savings'}</span><strong>{formatRupees(result.savings)}</strong></div></div>
    <div className="regime-grid"><RegimeCard title="Old Regime" data={result.oldRegime} recommended={recommendedOld}/><RegimeCard title="New Regime" data={result.newRegime} recommended={recommendedNew}/></div>
    {tied && <p className="tie-note">Both regimes result in the same tax. Choose the one that suits your financial needs.</p>}
  </div>
}
