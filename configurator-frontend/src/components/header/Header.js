import React from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { AppBar, Button, Grid, /*Button, */IconButton, Toolbar, Typography } from '@mui/material'
import { ArrowBackIosNew } from '@mui/icons-material'
import './Header.css'
import { Box } from '@mui/system'
import LanguageSelect from './LanguageSelect'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { connect } from 'react-redux'
import { dialogOpen } from '../../state/confirmationDialog/confirmationSlice'
import { resetActiveConfiguration } from '../../state/configuration/configurationSlice'

const usePathname = () => {
    const location = useLocation()
    return location.pathname
}

function Header({ language, open, resetConfig }) {

    const navigate = useNavigate()

    const onConfigurationPage = usePathname().split('/')[1] === 'configuration'

    function getMenuButtons() {
        return (
            <Grid container sx={{ flexGrow: 1, gap: 2 }}>

                <IconButton onClick={() => navigate('/')}>
                    <ArrowBackIosNew></ArrowBackIosNew>
                </IconButton>

                <Button 
                    sx={{display: {xs: onConfigurationPage ? 'block' : 'none'}}} 
                    variant="contained" 
                    onClick={() => resetConfig()}
                >
                    Reset
                </Button>

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

                        {/* <Button variant="contained" onClick={() => open('Example Message', {}, () => console.log('confirmed'))}>
                            test dialog
                        </Button> */}

                        <LanguageSelect></LanguageSelect>
                    </Toolbar>
                </AppBar>
            </Box>
        </header>
    )
}
const mapStateToProps = (state) => ({
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    // open: useConfirmationDialog.open
    open: dialogOpen,
    resetConfig: resetActiveConfiguration
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Header)
