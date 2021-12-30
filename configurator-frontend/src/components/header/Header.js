import React from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { AppBar, Button, Grid, /*Button, */IconButton, Toolbar, Typography } from '@mui/material'
import { AccountCircle, ArrowBackIosNew, Logout } from '@mui/icons-material'
import './Header.css'
import { Box } from '@mui/system'
import LanguageSelect from './LanguageSelect'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { connect } from 'react-redux'
// import { dialogOpen } from '../../state/confirmationDialog/confirmationSlice'
import { resetActiveConfiguration } from '../../state/configuration/configurationSlice'
import { selectIsAuthenticated, selectIsAdmin, selectUsername } from '../../state/user/userSelector'
import { logout } from '../../state/user/userSlice'
import { inputDialogOpen } from '../../state/inputDialog/inputDialogSlice'
import LoginButton from './LoginButton'
import RegisterButton from './RegisterButton'

const usePathname = () => {
    const location = useLocation()
    return location.pathname
}

function Header({ language, resetConfig, isLoggedIn, isAdmin, username, logout }) {

    const navigate = useNavigate()

    const onConfigurationPage = usePathname().split('/')[1] === 'configuration'

    const userButtons = (
        <>
            <Button 
                variant="contained" 
                onClick={() => console.log('save configuration pressed')}
                >
                Save Configuration
            </Button>
            <Button 
                variant="contained" 
                startIcon={<AccountCircle />}
                onClick={() => console.log('go to account page pressed')}
                >
                {username}
            </Button>
            <IconButton 
                variant="contained"
                onClick={() => logout()}
                >
                <Logout />
            </IconButton>
        </>
    )

    const adminButtons = (
        <>
            <Button
                variant="contained"
                onClick={() => console.log('create configuration pressed')}
            >
                Create Configuration
            </Button>
        </>
    )

    const guestButtons = (
        <>
            <LoginButton></LoginButton>
            <RegisterButton></RegisterButton>
        </>
    )

    function getMenuButtons() {
        return (
            <Grid container sx={{ gap: 2 }}>

                <IconButton onClick={() => navigate('/')}>
                    <ArrowBackIosNew></ArrowBackIosNew>
                </IconButton>

                <LanguageSelect></LanguageSelect>

                {onConfigurationPage ? 
                    <Button
                        // sx={{display: {xs: onConfigurationPage ? 'block' : 'none'}}} 
                        variant="contained" 
                        onClick={() => resetConfig()}
                    >
                        Reset
                    </Button>
                    : ''
                }

                <Box sx={{flexGrow: 1}}></Box>

                {/* <Button variant="contained" onClick={() => open('Example Message', {}, () => console.log('confirmed'))}>
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
    username: selectUsername(state)
})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen,
    resetConfig: resetActiveConfiguration,
    logout: logout
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Header)
