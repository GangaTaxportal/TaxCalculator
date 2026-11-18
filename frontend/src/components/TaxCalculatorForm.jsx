import { updateFormValue } from '../utils/updateFormValue.js'

const ageOptions = [['below60', 'Below 60 years'], ['60to79', '60 to 79 years'], ['80plus', '80 years and above']]

function MoneyField({ label, name, value, onChange, hint }) {
  return <label className="field"><span>{label}</span><div className="money-input"><span aria-hidden="true">₹</span><input type="number" min="0" step="1" inputMode="decimal" name={name} value={value} onChange={onChange} /></div>{hint && <small>{hint}</small>}</label>
}

function Section({ title, description, icon, children, className = '' }) {
  return <section className={`form-card ${className}`}><div className="card-heading"><div className="section-icon" aria-hidden="true">{icon}</div><div><h2>{title}</h2>{description && <p>{description}</p>}</div></div>{children}</section>
}

export default function TaxCalculatorForm({ values, onChange, onSubmit, onReset, loading }) {
  const update = (event) => {
    const { name, value } = event.target
    // Keep numeric input values as strings while editing so an empty field stays empty.
    onChange(updateFormValue(values, name, value))
  }
  return <form className="calculator-form" onSubmit={onSubmit}>
    <Section title="Basic details" description="A little about your tax profile" icon="◎" className="basic-card">
      <div className="field-grid"><label className="field"><span>Tax year</span><select name="taxYear" value={values.taxYear} onChange={update}><option value="2026-27">2026–27</option></select></label>
        <label className="field"><span>Age category</span><select name="ageCategory" value={values.ageCategory} onChange={update}>{ageOptions.map(([value, label]) => <option key={value} value={value}>{label}</option>)}</select></label></div>
    </Section>
    <Section title="Income details" description="Your income for the selected tax year" icon="↗" className="income-card">
      <div className="field-grid"><MoneyField label="Gross salary" name="grossSalary" value={values.grossSalary} onChange={update} hint="Before deductions and exemptions"/><MoneyField label="Salary allowances exempt under both regimes" name="salaryAllowancesExemptBothRegimes" value={values.salaryAllowancesExemptBothRegimes} onChange={update}/><MoneyField label="Other income" name="otherIncome" value={values.otherIncome} onChange={update} hint="Interest, dividends, and other sources"/><MoneyField label="House property interest" name="housePropertyInterest" value={values.housePropertyInterest} onChange={update}/></div>
    </Section>
    <div className="deduction-grid"><Section title="Old regime deductions" description="Eligible deductions under the old regime" icon="−"><div className="field-stack"><MoneyField label="Section 80C" name="oldRegimeDeductions.section80C" value={values.oldRegimeDeductions.section80C} onChange={update}/><MoneyField label="Section 80D" name="oldRegimeDeductions.section80D" value={values.oldRegimeDeductions.section80D} onChange={update}/><MoneyField label="Other deductions" name="oldRegimeDeductions.otherDeductions" value={values.oldRegimeDeductions.otherDeductions} onChange={update}/></div></Section>
      <Section title="New regime deductions" description="Eligible deductions under the new regime" icon="−"><div className="field-stack"><MoneyField label="New regime deductions" name="newRegimeDeductions" value={values.newRegimeDeductions} onChange={update} hint="Enter the total eligible amount"/></div></Section></div>
    <div className="form-actions"><button type="button" className="button button-secondary" onClick={onReset} disabled={loading}>Reset</button><button type="submit" className="button button-primary" disabled={loading}>{loading ? <><span className="button-spinner"/> Comparing…</> : <>Compare tax <span aria-hidden="true">→</span></>}</button></div>
  </form>
}
