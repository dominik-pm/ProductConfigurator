import React from 'react'
import { createTheme, ThemeProvider } from '@mui/material/styles'
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

const theme = createTheme({
})

function App() {
    return (
        <ThemeProvider theme={theme}>
            <div className="App">
                <Router>
                    <Header></Header>
                    <ConfirmationOptionSelect></ConfirmationOptionSelect>
                    <InputDialog></InputDialog>

                    <Routes>

                        <Route exact path="/" element={
                            <ProductView></ProductView>
                        }>
                        </Route>

                        <Route exact path="/configuration/:id" element={
                            <ConfigurationView></ConfigurationView>
                        }>
                        </Route>

                    </Routes>
                </Router>
            </div>
        </ThemeProvider>
    )
}

export default App
