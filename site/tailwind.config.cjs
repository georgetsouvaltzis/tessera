/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/**/*.{js,jsx,ts,tsx,md,mdx}',
    './docs/**/*.{md,mdx}',
    './docusaurus.config.ts',
    './sidebars.ts',
  ],
  theme: {
    extend: {
      colors: {
        background: 'oklch(0.14 0.025 340 / <alpha-value>)',
        foreground: 'oklch(0.97 0.01 340 / <alpha-value>)',
        card: 'oklch(0.18 0.03 340 / <alpha-value>)',
        secondary: 'oklch(0.22 0.04 340 / <alpha-value>)',
        muted: 'oklch(0.22 0.03 340 / <alpha-value>)',
        'muted-foreground': 'oklch(0.7 0.04 340 / <alpha-value>)',
        border: 'oklch(0.28 0.04 340 / <alpha-value>)',
        primary: 'oklch(0.78 0.18 350 / <alpha-value>)',
        'primary-glow': 'oklch(0.85 0.2 350 / <alpha-value>)',
        'primary-foreground': 'oklch(0.14 0.025 340 / <alpha-value>)',
      },
    },
  },
  plugins: [],
};
