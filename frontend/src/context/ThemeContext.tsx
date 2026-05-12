import { useEffect, useMemo, useState, type ReactNode } from 'react'

import type { Theme } from '../types/theme'
import { ThemeContext } from './themeContextValue'

function readInitialTheme(): Theme {
  try {
    const stored = localStorage.getItem('app.theme')
    if (stored === 'light' || stored === 'dark') return stored
  } catch {
    // localStorage not available (private mode)
  }
  return 'light'
}

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [theme, setThemeState] = useState<Theme>(readInitialTheme)

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme)
    try {
      localStorage.setItem('app.theme', theme)
    } catch {
      // localStorage not available
    }
  }, [theme])

  const value = useMemo(() => ({
    theme,
    setTheme: (t: Theme) => setThemeState(t),
    toggleTheme: () => setThemeState(prev => (prev === 'light' ? 'dark' : 'light')),
  }), [theme])

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>
}
