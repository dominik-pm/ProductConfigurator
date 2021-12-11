import React from 'react'
import { createTheme, ThemeProvider } from '@mui/material/styles'
import ProductView from './components/products/ProductView'

const theme = createTheme({

})

function App() {
    return (
        <ThemeProvider theme={theme}>
            <div className="App">
                <ProductView></ProductView>
            </div>
        </ThemeProvider>
    )
}

export default App
