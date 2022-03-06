import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { /*Accordion, AccordionDetails, AccordionSummary,*/ AppBar, Button, Drawer, Grid, /*Button, */IconButton, Toolbar, Tooltip, Typography } from '@mui/material'
import { AccountCircle, /*ArrowBackIosNew, ExpandMoreOutlined,*/ Home, Logout, Menu } from '@mui/icons-material'
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
import { fetchProducts } from '../../state/product/productSlice'

function Header({ language, isLoggedIn, isAdmin, username, logout, fetchProducts }) {

    const navigate = useNavigate()

    const [drawerOpen, setDrawerOpen] = useState(false)

    const [mobileView, setMobileView] = useState(false)
    useEffect(() => {
        const setResponsiveness = () => {
            return window.innerWidth < 900
                ? setMobileView(true)
                : setMobileView(false)
        }

        setResponsiveness();
        window.addEventListener('resize', () => setResponsiveness())

        return () => {
            window.removeEventListener('resize', () => setResponsiveness())
        }
    }, [])

    const handleHomePressed = () => {
        navigate('/')
        fetchProducts()
    }

    const userButtons = (
        <>
            <Button
                size="large"
                variant="contained"
                startIcon={<AccountCircle />}
                onClick={() => navigate('/user')}
            >
                <Typography variant="body1" sx={{maxWidth: '180px', overflow: 'hidden', textOverflow: 'ellipsis'}}>
                    {username}
                </Typography>
            </Button>
            <Tooltip title={translate('logout', language)}>
                <IconButton
                    size="large"
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
                size="large"
                variant="contained"
                onClick={handleCreateConfigPressed}
            >
                {translate('createConfiguration', language)}
            </Button>
        </>
    )

    const guestButtons = (
        <>
            <LoginButton size="large"></LoginButton>
            <RegisterButton size="large"></RegisterButton>
        </>
    )

    const homeButton = (
        <>
            <IconButton onClick={handleHomePressed}>
                <Home sx={{color: 'white'}} />
            </IconButton>
        </>
    )

    function handleCreateConfigPressed() {
        navigate('/create')
    }

    function getMenuButtons() {
        return (
            // <Grid container sx={{ gap: 2 }} >
            <>
                {/* <LanguageSelect></LanguageSelect> */}

                {/* <Box sx={{ flexGrow: 1 }}></Box> */}

                {/* <Button variant="contained" onClick={() => openConfirmDialog('Example Message', {}, null, () => console.log('confirmed'))}>
                    test dialog
                </Button> */}

                {isAdmin ? adminButtons : ''}

                {isLoggedIn ? userButtons : guestButtons}

            </>
            // </Grid>
        )
    }

    function handleToggleDrawer(on) {
        setDrawerOpen(on)
    }

    function mobileToolbar() {
        // const drawerBleeding = 56

        return (
            // <SwipeableDrawer
            //     anchor="top"
            //     open={drawerOpen}
            //     // onClose={handleToggleDrawer(false)}
            //     // onOpen={handleToggleDrawer(true)}
            //     swipeAreaWidth={drawerBleeding}
            //     disableSwipeToOpen={false}
            //     ModalProps={{
            //         keepMounted: true,
            //     }}
            // >
            //     <Box
            //         sx={{
            //             position: 'absolute',
            //             top: -drawerBleeding,
            //             borderTopLeftRadius: 8,
            //             borderTopRightRadius: 8,
            //             visibility: 'visible',
            //             right: 0,
            //             left: 0,
            //         }}
            //     >
            //         <Menu fontSize="large" />
            //         <Typography sx={{ p: 2, color: 'text.secondary' }}>Open Menu</Typography>
            //     </Box>
            //     <Box
            //         sx={{
            //             px: 2,
            //             pb: 2,
            //             height: '100%',
            //             overflow: 'auto',
            //         }}
            //     >

            //     <Box sx={{ flexGrow: 1 }}></Box>

            //     <IconButton onClick={() => navigate('/')}>
            //         <ArrowBackIosNew></ArrowBackIosNew>
            //     </IconButton>

            //     <LanguageSelect></LanguageSelect>
            //     </Box>
            // </SwipeableDrawer>

            <AppBar position="static">
                <Toolbar>

                    <Grid container direction="row">

                        <IconButton
                            size="large"
                            edge="start"
                            color="inherit"
                            aria-label="menu"
                            sx={{ mr: 2 }}
                            onClick={() => handleToggleDrawer(true)}
                            >
                            <Menu fontSize="large" />
                        </IconButton>

                        {homeButton}
                        
                        <Box sx={{ flexGrow: 1 }}></Box>
                        
                        <LanguageSelect></LanguageSelect>
                    
                    </Grid>

                    <Drawer
                        anchor="top"
                        open={drawerOpen}
                        onClose={() => handleToggleDrawer(false)}
                    >
                        <Grid container direction="column" padding={5} gap={2}>

                            {getMenuButtons()}

                        </Grid>
                    </Drawer>

                </Toolbar>
            </AppBar>

            // <Accordion expanded={drawerOpen}>
            //     <AccordionSummary
            //         expandIcon={<ExpandMoreOutlined />}
            //         aria-controls="panel1a-content"
            //         id="panel1a-header"
            //         onClick={() => handleToggleDrawer(!drawerOpen)}
            //     >

            //         <IconButton
            //             size="large"
            //             edge="start"
            //             color="inherit"
            //             aria-label="menu"
            //             sx={{ mr: 2 }}
            //             onClick={() => handleToggleDrawer(!drawerOpen)}
            //         >
            //             <Menu fontSize="large" />
            //         </IconButton>

            //         {/* <Box sx={{ flexGrow: 1 }}></Box>
                    
            //         <IconButton onClick={() => navigate('/')}>
            //             <ArrowBackIosNew></ArrowBackIosNew>
            //         </IconButton> */}


            //     </AccordionSummary>
            //     <AccordionDetails>

            //         <Grid container direction="column" gap={2} alignItems="center">
            //             <LanguageSelect></LanguageSelect>

            //             {isAdmin ? adminButtons : ''}

            //             {isLoggedIn ? userButtons : guestButtons}
            //         </Grid>

            //     </AccordionDetails>
            // </Accordion >
        )
    }

    function desktopToolbar() {
        return (
            <AppBar position="static">
                <Toolbar variant="regular">
                    <Grid justifyContent="flex-end" container gap={2}>
                        {homeButton}

                        <LanguageSelect></LanguageSelect>

                        <Box sx={{ flexGrow: 1 }}></Box>

                        {/* <Grid container justifyContent="flex-end" gap={2}> */}
                            {getMenuButtons()}
                        {/* </Grid> */}
                    </Grid>
                </Toolbar>
            </AppBar>
        )
    }

    return (
        <header>
            <Box sx={{ flexGrow: 1 }}>
                <Typography align='center' marginBottom={1} variant='h1'>
                    {translate('productConfigurator', language)}
                </Typography>

                {mobileView ? mobileToolbar() : desktopToolbar()}
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
    fetchProducts: fetchProducts
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Header)
