import { alertStatus } from './alertSlice'

export const selectIsAlertOpen = (state) =>         state.alert.status === alertStatus.OPEN
export const selectCurrentAlert = (state) =>        state.alert.alerts[0]