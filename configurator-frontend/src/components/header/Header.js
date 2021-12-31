import React from 'react'
import { useNavigate } from 'react-router-dom'
import { AppBar, Button, Grid, /*Button, */IconButton, Toolbar, Typography } from '@mui/material'
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

function Header({ language, isLoggedIn, isAdmin, username, logout }) {

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

                <Box sx={{flexGrow: 1}}></Box>

                {/* <Button variant="contained" onClick={() => openConfirmDialog('Example Message', {}, () => console.log('confirmed'))}>
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
    logout: logout
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Header)
