import React from 'react'
import { useNavigate } from 'react-router-dom'
import { AppBar, Button, Grid, /*Button, */IconButton, Toolbar, Tooltip, Typography } from '@mui/material'
import { AccountCircle, ArrowBackIosNew, Logout } from '@mui/icons-material'
import './Header.css'
import { Box } from '@mui/system'
import LanguageSelect from './LanguageSelect'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { connect } from 'react-redux'
import { selectIsAuthenticated, selectIsAdmin, selectUserName } from '../../state/user/userSelector'
import { logout } from '../../state/user/userSlice'
import { inputDialogOpen } from '../../state/inputDialog/inputDialogSlice'
import LoginButton from './LoginButton'
import RegisterButton from './RegisterButton'
import { alertTypes, openAlert } from '../../state/alert/alertSlice'

function Header({ language, isLoggedIn, isAdmin, username, logout, openAlert }) {

    const navigate = useNavigate()

    const userButtons = (
        <>
            <Button 
                variant="contained" 
                startIcon={<AccountCircle />}
                onClick={() => navigate('/account')}
                >
                {username}
            </Button>
            <Tooltip title={translate('logout', language)}>
                <IconButton 
                    variant="contained"
                    onClick={() => logout()}
                    >
                        <Logout />
                </IconButton>
            </Tooltip>
        </>
    )

    const adminButtons = (
        <>
            <Button
                variant="contained"
                onClick={handleCreateConfigPressed}
            >
                {translate('createConfiguration', language)}
            </Button>
        </>
    )

    const guestButtons = (
        <>
            <LoginButton></LoginButton>
            <RegisterButton></RegisterButton>
        </>
    )

    function handleCreateConfigPressed() {
        openAlert('Not implemented', alertTypes.WARNING)
    }

    function getMenuButtons() {
        return (
            <Grid container sx={{ gap: 2 }} >

                <IconButton onClick={() => navigate('/')}>
                    <ArrowBackIosNew></ArrowBackIosNew>
                </IconButton>

                <LanguageSelect></LanguageSelect>

                <Box sx={{flexGrow: 1}}></Box>

                {/* <Button variant="contained" onClick={() => openConfirmDialog('Example Message', {}, null, () => console.log('confirmed'))}>
                    test dialog
                </Button> */}

                {isAdmin ? adminButtons : ''}

                {isLoggedIn ? userButtons : guestButtons}

            </Grid>
        )
    }

    return (
        <header>
            {/* <AppBar>
                <Toolbar className='header-container'>
                    {getMenuButtons()}
                    <div className="lang-select">
                        langselect
                    </div>
                </Toolbar>
            </AppBar> */}
            <Box sx={{ flexGrow: 1 }}>
                <Typography align='center' marginBottom={1} variant='h1'>
                    {translate('productConfigurator', language)}
                </Typography>
                <AppBar position="static">
                    <Toolbar>
                        {/* <IconButton
                            size="large"
                            edge="start"
                            color="inherit"
                            aria-label="menu"
                            sx={{ mr: 2 }}
                        >
                            <MenuIcon />
                        </IconButton> */}
                        {getMenuButtons()}

                    </Toolbar>
                </AppBar>
            </Box>
        </header>
    )
}
const mapStateToProps = (state) => ({
    language: selectLanguage(state),
    isLoggedIn: selectIsAuthenticated(state),
    isAdmin: selectIsAdmin(state),
    username: selectUserName(state)
})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen,
    logout: logout,
    openAlert
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Header)
