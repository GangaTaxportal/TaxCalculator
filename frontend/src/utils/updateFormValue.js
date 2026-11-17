export function updateFormValue(values, name, value) {
  if (name.startsWith('oldRegimeDeductions.')) {
    const key = name.split('.')[1]
    return {
      ...values,
      oldRegimeDeductions: { ...values.oldRegimeDeductions, [key]: value },
    }
  }
  return { ...values, [name]: value }
}
