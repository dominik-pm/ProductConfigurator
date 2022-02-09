import React from 'react'
import {
    BrowserRouter as Router,
    Route,
    Routes
} from 'react-router-dom'
import { ThemeProvider } from '@mui/material/styles'
import { Container, Typography } from '@mui/material'
import Header from './components/header/Header'
import ConfirmationOptionSelect from './components/dialog/ConfirmationDialog'
import InputDialog from './components/dialog/InputDialog'
import GenericAlert from './components/alert/GenericAlert'
import ProductView from './components/products/ProductView'
import AccountView from './components/account/AccountView'
import ConfigurationBuilderView from './components/configuration/Builder/ConfigurationBuilderView'
import ConfigurationView from './components/configuration/Configurator/ConfigurationView'
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

                        <Route exact path="/create" element={
                            <ConfigurationBuilderView></ConfigurationBuilderView>
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

export function getImageSource(image) {
    let imageSource = ''

    try {
        const src = require(`./assets/img/${image.replace('./', '')}`)
        imageSource = src.default
    } catch (err) {
        console.log(`image '${image}' no found!`)
        const src = require(`./assets/img/notfound.jpg`)
        imageSource = src.default
    } finally {
        return imageSource
    }
}

export function writeToLocalStorage(data, key) {
    try {
        data = JSON.stringify(data)
        localStorage.setItem(key, data)
        // console.log('Saved to storage!')
        // console.log(data)
    } catch(err) {
        console.log('Can not save the to local storage!')
        console.log(err)
    }
}

export function readFromLocalStorage(key) {
    let data = null
    try {
        data = JSON.parse(localStorage.getItem(key))
    } catch(err) {
        console.log('Can not load from local storage!')
        console.log(err)
    }

    return data
}

export default App