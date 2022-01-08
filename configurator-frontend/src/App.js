import React from 'react'
import { ThemeProvider } from '@mui/material/styles'
import ProductView from './components/products/ProductView'
import ConfigurationView from './components/configuration/ConfigurationView'
import Header from './components/header/Header'
import {
    BrowserRouter as Router,
    Route,
    Routes
} from 'react-router-dom'
import ConfirmationOptionSelect from './components/dialog/ConfirmationDialog'
import InputDialog from './components/dialog/InputDialog'
import AccountView from './components/account/AccountView'
import GenericAlert from './components/alert/GenericAlert'
import { Container, Typography } from '@mui/material'
import theme from './Theme'

function App() {
    return (
        <ThemeProvider theme={theme}>
            <Container sx={{padding: 0}} maxWidth="xl" className="App">
                <Router>
                    <Header></Header>
                    <ConfirmationOptionSelect></ConfirmationOptionSelect>
                    <InputDialog></InputDialog>
                    <GenericAlert></GenericAlert>

                    <Routes>

                        <Route exact path="/" element={
                            <ProductView></ProductView>
                        }>
                        </Route>

                        <Route exact path="/account/:tab" element={<AccountView></AccountView>}></Route>
                        <Route exact path="/account" element={<AccountView></AccountView>}></Route>

                        <Route exact path="/configuration/:id" element={
                            <ConfigurationView></ConfigurationView>
                        }>
                        </Route>

                        <Route path="*" element={
                            <Typography variant="h2">Not Found!</Typography>
                        }>
                        </Route>

                    </Routes>
                </Router>
            </Container>
        </ThemeProvider>
    )
}

export default App
