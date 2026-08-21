import { useEffect, useMemo, useState } from 'react'
import type {
  OnboardingChecklistProgress,
  OnboardingChecklistProgressContext,
  OnboardingChecklistProgressStore,
} from './api'

export interface OnboardingChecklistStep {
  id: string
  title: string
  description: string
}

export interface OnboardingChecklistStepState extends OnboardingChecklistStep {
  completed: boolean
}

export interface OnboardingChecklistState {
  isLoaded: boolean
  steps: OnboardingChecklistStepState[]
  completedCount: number
  totalCount: number
  toggleStep: (stepId: string) => void
}

const STORAGE_PREFIX = 'cp:onboarding:v1'

function getStorageKey(context: OnboardingChecklistProgressContext): string {
  return `${STORAGE_PREFIX}:${context.contractorId}:${context.teamMemberId}`
}

function canUseStorage(): boolean {
  return typeof window !== 'undefined' && !!window.localStorage
}

function parseProgress(raw: string | null): OnboardingChecklistProgress | null {
  if (!raw) {
    return null
  }

  try {
    const parsed = JSON.parse(raw) as Partial<OnboardingChecklistProgress>
    if (!Array.isArray(parsed.completedStepIds) || typeof parsed.updatedAtUtc !== 'string') {
      return null
    }

    return {
      completedStepIds: parsed.completedStepIds.filter((value): value is string => typeof value === 'string'),
      updatedAtUtc: parsed.updatedAtUtc,
    }
  } catch {
    return null
  }
}

export const localOnboardingChecklistProgressStore: OnboardingChecklistProgressStore = {
  async load(context) {
    if (!canUseStorage()) {
      return null
    }

    const key = getStorageKey(context)
    const raw = window.localStorage.getItem(key)
    return parseProgress(raw)
  },

  async save(context, progress) {
    if (!canUseStorage()) {
      return
    }

    const key = getStorageKey(context)
    window.localStorage.setItem(key, JSON.stringify(progress))
  },
}

export function useOnboardingChecklist(
  context: OnboardingChecklistProgressContext,
  steps: OnboardingChecklistStep[],
  store: OnboardingChecklistProgressStore = localOnboardingChecklistProgressStore
): OnboardingChecklistState {
  const contextKey = `${context.contractorId}:${context.teamMemberId}`
  const [loadedContextKey, setLoadedContextKey] = useState<string | null>(null)
  const [completedStepIds, setCompletedStepIds] = useState<string[]>([])

  useEffect(() => {
    let mounted = true
    const loadContext: OnboardingChecklistProgressContext = {
      contractorId: context.contractorId,
      teamMemberId: context.teamMemberId,
    }

    const load = async () => {
      const progress = await store.load(loadContext)
      if (!mounted) {
        return
      }

      const validStepIds = new Set(steps.map((step) => step.id))
      const filtered = (progress?.completedStepIds ?? []).filter((stepId) => validStepIds.has(stepId))

      setCompletedStepIds(filtered)
      setLoadedContextKey(contextKey)
    }

    void load()

    return () => {
      mounted = false
    }
  }, [context.contractorId, context.teamMemberId, contextKey, steps, store])

  const completedStepIdSet = useMemo(() => new Set(completedStepIds), [completedStepIds])

  const stepStates = useMemo(
    () =>
      steps.map((step) => ({
        ...step,
        completed: completedStepIdSet.has(step.id),
      })),
    [completedStepIdSet, steps]
  )

  const toggleStep = (stepId: string) => {
    setCompletedStepIds((previous) => {
      const nextSet = new Set(previous)

      if (nextSet.has(stepId)) {
        nextSet.delete(stepId)
      } else {
        nextSet.add(stepId)
      }

      const next = Array.from(nextSet)
      void store.save(context, {
        completedStepIds: next,
        updatedAtUtc: new Date().toISOString(),
      })

      return next
    })
  }

  return {
    isLoaded: loadedContextKey === contextKey,
    steps: stepStates,
    completedCount: completedStepIds.length,
    totalCount: steps.length,
    toggleStep,
  }
}
