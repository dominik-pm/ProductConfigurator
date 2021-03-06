import React from 'react'
import { Button } from '@mui/material'
import { connect } from 'react-redux'
import { inputDialogOpen } from '../../state/inputDialog/inputDialogSlice'
import { login } from '../../state/user/userSlice'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'

export const openLogInDialog = () => (dispatch) => {
    const data = {
        username: {name: 'Username', value: ''},
        password: {name: 'Password', value: '', isPassword: true}
    }
    dispatch(inputDialogOpen('Login', data, (data) => {
        dispatch(login(data.username.value, data.username.password))
    }))
}

function LoginButton({ openLogin, language, size = 'small' }) {


    return (
        <Button
            size={size}
            variant="contained" 
            onClick={openLogin}
            >
            {translate('login', language)}
        </Button>
    )
}

const mapStateToProps = (state) => ({
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    openLogin: openLogInDialog
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(LoginButton)