const configuredApiUrl = import.meta.env.VITE_API_URL || ''

export function apiUrl(path) {
  if (!configuredApiUrl) {
    return path
  }

  const baseUrl = configuredApiUrl.replace(/\/$/, '')
  const normalizedPath = path.replace(/^\//, '')

  return `${baseUrl}/${normalizedPath}`
}
