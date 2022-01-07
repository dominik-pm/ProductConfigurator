import React from 'react'
import { Button } from '@mui/material'
import { connect } from 'react-redux'
import { inputDialogOpen } from '../../state/inputDialog/inputDialogSlice'
import { register } from '../../state/user/userSlice'
import { alertTypes, openAlert } from '../../state/alert/alertSlice'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'

function RegisterButton({ openInputDialog, register, openAlert, language }) {

    function openRegisterDialog() {
        const data = {
            username: {name: 'Username', value: ''},
            email: {name: 'Email', value: '', isEmail: true},
            password: {name: 'Password', value: '', isPassword: true},
            confirmPassword: {name: 'Confirm Password', value: '', isPassword: true}
        }
        openInputDialog('Register', data, (data) => {
            if (data.confirmPassword.value !== data.password.value) {
                openAlert(`${translate('passwordDontMatch', language)}!`, alertTypes.ERROR)
                return
            }
            register(data.username.value, data.email.value, data.username.password)
        })
    }

    return (
        <Button 
            variant="contained" 
            onClick={openRegisterDialog}
            >
            Register
        </Button>
    )
}

const mapStateToProps = (state) => ({
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen,
    register: register,
    openAlert: openAlert
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(RegisterButton)