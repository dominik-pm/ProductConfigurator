import React from 'react'
import { useNavigate } from 'react-router-dom'
import { AppBar, /*Button, */IconButton, Toolbar, Typography } from '@mui/material'
import { ArrowBackIosNew } from '@mui/icons-material'
import './Header.css'
import { Box } from '@mui/system'
import LanguageSelect from './LanguageSelect'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { connect } from 'react-redux'

function Header({ language }) {

    const navigate = useNavigate()

    function getMenuButtons() {
        return (
            <Box sx={{ flexGrow: 1 }}>
                <IconButton onClick={() => navigate('/')}>
                    <ArrowBackIosNew></ArrowBackIosNew>
                </IconButton>
            </Box>
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
const mapDispatchToProps = {}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Header)
