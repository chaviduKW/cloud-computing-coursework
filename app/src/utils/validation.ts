import * as yup from 'yup'
import type { Rule } from 'antd/es/form'

// ── Schemas ──────────────────────────────────────────────────────────────────

export const loginSchema = yup.object({
  email: yup
    .string()
    .email('Please enter a valid email address')
    .required('Email is required'),
  password: yup
    .string()
    .required('Password is required'),
})

export const registerSchema = yup.object({
  firstName: yup
    .string()
    .min(2, 'First name must be at least 2 characters')
    .required('First name is required'),
  lastName: yup
    .string()
    .min(2, 'Last name must be at least 2 characters')
    .required('Last name is required'),
  email: yup
    .string()
    .email('Please enter a valid email address')
    .required('Email is required'),
  password: yup
    .string()
    .min(8, 'Password must be at least 8 characters')
    .matches(/[A-Z]/, 'Password must contain at least one uppercase letter')
    .matches(/[0-9]/, 'Password must contain at least one number')
    .required('Password is required'),
  confirmPassword: yup
    .string()
    .oneOf([yup.ref('password')], 'Passwords do not match')
    .required('Please confirm your password'),
})

export type LoginFormValues = yup.InferType<typeof loginSchema>
export type RegisterFormValues = yup.InferType<typeof registerSchema>

// ── Ant Design Rule Helper ────────────────────────────────────────────────────

/**
 * Converts a yup field schema into an Ant Design Form rule for inline validation.
 */
export function yupRule(fieldSchema: yup.Schema | yup.Reference): Rule {
  return {
    validator: async (_: unknown, value: unknown) => {
      if (!('validate' in fieldSchema)) return
      try {
        await (fieldSchema as yup.Schema).validate(value)
      } catch (err) {
        if (err instanceof yup.ValidationError) {
          throw new Error(err.message)
        }
        throw err
      }
    },
  }
}
