import { Moon, Sun } from 'lucide-react'

import { useTheme } from '../context/themeContextValue'

export default function ThemeToggle() {
  const { theme, toggleTheme } = useTheme()
  const isDark = theme === 'dark'

  return (
    <button
      className="theme-toggle"
      role="switch"
      aria-checked={isDark}
      aria-label={isDark ? 'Schakel naar licht thema' : 'Schakel naar donker thema'}
      onClick={toggleTheme}
    >
      <span className="theme-toggle-track">
        <span className="theme-toggle-thumb">
          {isDark ? <Moon size={13} /> : <Sun size={13} />}
        </span>
      </span>
      <span className="theme-toggle-label">{isDark ? 'Donker' : 'Licht'}</span>
    </button>
  )
}
