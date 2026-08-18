---
name: ContractorPro
description: Clean professional contractor SaaS — planning-stage visual identity for SME validation mockups.
status: draft
updated: 2026-08-18
colors:
  primary: '#1B4B7A'
  primary-foreground: '#FFFFFF'
  accent: '#E87722'
  accent-foreground: '#FFFFFF'
  background: '#F4F6F8'
  surface: '#FFFFFF'
  foreground: '#1A2332'
  muted: '#6B7A8D'
  border: '#D8DEE6'
  success: '#2D8A4E'
  warning: '#D4A012'
  danger: '#C0392B'
typography:
  fontFamily: "'Segoe UI', system-ui, -apple-system, sans-serif"
  headingWeight: '600'
  bodySize: '15px'
rounded:
  sm: 6px
  md: 10px
  lg: 14px
spacing:
  unit: 8px
components:
  button-primary:
    background: '{colors.primary}'
    foreground: '{colors.primary-foreground}'
    radius: '{rounded.md}'
  button-accent:
    background: '{colors.accent}'
    foreground: '{colors.accent-foreground}'
    radius: '{rounded.md}'
  status-pending:
    background: '#FFF8E6'
    foreground: '#8B6914'
    border: '#F0D878'
  status-confirmed:
    background: '#E8F5EC'
    foreground: '{colors.success}'
    border: '#A8D5B5'
---

## Brand & Style

ContractorPro reads as **clean, professional contractor SaaS** — trustworthy enough for office managers, simple enough for field subs who never log in. No logo lockup yet; wordmark placeholder only. Visual restraint: blue for structure, orange for action, green/amber for status.

## Colors

- **Primary Blue** — nav, headers, primary buttons, links
- **Accent Orange** — CTAs that need attention (Confirm cascade, Send reminder)
- **Status tokens** — pending (amber), confirmed (green), declined (red)

## Typography

System sans-serif stack. Headings semibold; body 15px. No display serif — this is operational software, not marketing.

## Layout & Spacing

8px grid. Desktop max content ~1200px with sidebar nav. Mobile full-bleed with 16px horizontal padding.

## Components

Mockups illustrate: sidebar nav, project cards, pending badges, cascade preview panel, MMS thread mirror, magic-link accept screens, customer timeline cards.

## Do's and Don'ts

- **Do** show real project names (Maple St Kitchen), real task names, real SMS copy
- **Do** keep sub/customer portals minimal — one primary action per screen
- **Don't** use lorem ipsum or generic "User" labels
- **Don't** show subs in customer views or customer in sub views
