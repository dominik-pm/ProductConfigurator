export const selectIsAuthenticated = state =>       state.user.isAuthenticated
export const selectIsAdmin = state =>               state.user.user.isAdmin || false
export const selectUsername = state =>              selectCurrentUser(state).username || ''

const selectCurrentUser = state =>        state.user.user