export const selectIsAuthenticated = state =>       state.user.isAuthenticated
export const selectIsAdmin = state =>               state.user.user.admin || false
export const selectUserName = state =>              selectCurrentUser(state).username || ''
export const selectUserEmail = state =>             selectCurrentUser(state).email || ''

export const selectSavedConfigurations = state =>       state.user.savedConfigurations
export const selectOrderedConfigurations = state =>     state.user.orderedConfigurations
export const selectAllOrderedConfigurations = state =>  state.user.allOrderedConfigurations

const selectCurrentUser = state =>        state.user.user