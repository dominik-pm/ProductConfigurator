export const selectIsAuthenticated = state =>       state.user.isAuthenticated
export const selectIsAdmin = state =>               state.user.user.admin || false
export const selectUserName = state =>              selectCurrentUser(state).userName || ''
export const selectUserEmail = state =>             selectCurrentUser(state).userEmail || ''

export const selectSavedConfigurations = state =>       state.user.savedConfigurations
export const selectOrderedConfigurations = state =>     state.user.orderedConfigurations
export const selectAllOrderedConfigurations = state =>  state.user.allOrderedConfigurations

const selectCurrentUser = state =>        state.user.user

export const extractUsernameFromConfiguration = (configuration) =>      configuration.user?.userName || ''
export const extractEmailFromConfiguration = (configuration) =>         configuration.user?.userEmail || ''

export const extractNameFromConfiguration = (configuration) =>          configuration.savedName
export const extractIdFromConfiguration = (configuration) =>            configuration.configId
export const extractOptionsFromConfiguration = (configuration) =>       configuration.options
export const extractDateFromConfiguration = (configuration) =>          configuration.date