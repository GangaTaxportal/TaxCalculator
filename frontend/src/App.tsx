import { useState } from 'react'
import type { FormEvent } from 'react'
import { compareTax, TaxApiError } from './Api/taxApi'
import TaxCalculatorForm from './components/TaxCalculatorForm'
import ComparisonResults from './components/ComparisonResults'
import { createInitialValues } from './utils/initialValues'
import { toTaxRequest } from './utils/taxRequest'
import type { CalculatorFormValues, TaxComparisonData } from './types/tax'
import './App.css'

function App() {
  const [values, setValues] = useState<CalculatorFormValues>(createInitialValues)
  const [formVersion, setFormVersion] = useState(0)
  const [result, setResult] = useState<TaxComparisonData | null>(null)
  const [loading, setLoading] = useState(false)
  const [errors, setErrors] = useState<string[]>([])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErrors([])
    setLoading(true)
    try {
      setResult(await compareTax(toTaxRequest(values)))
    } catch (error: unknown) {
      setResult(null)
      if (error instanceof TaxApiError) {
        setErrors(error.messages)
      } else if (error instanceof Error) {
        setErrors([error.message || 'Unable to connect to the tax service. Please try again.'])
      } else {
        setErrors(['Unable to connect to the tax service. Please try again.'])
      }
    } finally {
      setLoading(false)
    }
  }

  function handleReset() {
    setValues(createInitialValues())
    setFormVersion((version) => version + 1)
    setResult(null)
    setErrors([])
  }

  return (
    <main className="app-shell">
      <header className="page-header">
        <div className="brand-mark" aria-hidden="true">₹</div>
        <div><p className="eyebrow">PERSONAL FINANCE</p><h1>Income Tax Calculator</h1>
          <p className="subtitle">Compare your tax liability under Old and New Tax Regimes.</p></div>
      </header>
      <div className="layout-grid">
        <TaxCalculatorForm key={formVersion} values={values} onChange={setValues} onSubmit={handleSubmit} onReset={handleReset} loading={loading} />
        <section className="results-area" aria-live="polite">
          {errors.length > 0 && <div className="error-banner" role="alert"><strong>We couldn’t calculate your tax</strong>{errors.map((error, index) => <p key={`${error}-${index}`}>{error}</p>)}</div>}
          {loading ? <div className="empty-state loading-state"><span className="spinner" /><h2>Comparing your regimes</h2><p>Calculating your tax liability…</p></div>
            : result ? <ComparisonResults result={result} />
              : <div className="empty-state"><div className="empty-icon" aria-hidden="true">↗</div><h2>Your comparison will appear here</h2><p>Enter your income and deductions, then select <strong>Compare tax</strong> to see a personalized breakdown.</p><div className="empty-note"><span>✓</span> A clear, side-by-side view of both regimes</div></div>}
        </section>
      </div>
      <footer>For a quick estimate only · Verify your final tax liability with a tax professional</footer>
    </main>
  )
}

export default App
