import { createSlice } from '@reduxjs/toolkit'
import { requestLogin, setAuthorizationToken } from '../../api/userAPI'

const initialState = {
    isAuthenticated: false,
    user: {}
}

export const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        setCurrentUser: (state, action) => {
            console.log('getting user:', action.payload)
            state.isAuthenticated = !action.payload
            state.user = action.payload
        }
    }
})

export const login = (username, password) => async (dispatch) => {
    
    requestLogin(username, password).then(res => {
        const { token, user } = res

        localStorage.setItem('jwtToken', token)
        setAuthorizationToken(token)
        dispatch(setCurrentUser(user))
    })
    .catch(err => {
        console.log(err)
    })
}

export const logout = () => (dispatch) => {
    localStorage.removeItem('jwtToken')
    setAuthorizationToken(false)
    dispatch(setCurrentUser({}))
}

// Action creators are generated for each case reducer function
export const { setCurrentUser } = userSlice.actions

export default userSlice.reducer