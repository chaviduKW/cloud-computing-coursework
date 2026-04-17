import { App } from 'antd'

/**
 * Returns notification helpers (success / error / info / warning)
 * that appear at bottom-right, driven by AntdApp context.
 */
export function useToast() {
  const { notification } = App.useApp()

  return {
    success: (message: string, description?: string) =>
      notification.success({ message, description, placement: 'bottomRight' }),
    error: (message: string, description?: string) =>
      notification.error({ message, description, placement: 'bottomRight' }),
    info: (message: string, description?: string) =>
      notification.info({ message, description, placement: 'bottomRight' }),
    warning: (message: string, description?: string) =>
      notification.warning({ message, description, placement: 'bottomRight' }),
  }
}
