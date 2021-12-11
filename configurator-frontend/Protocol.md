# Product Configurator Frontend

## Dependency Setup
**Creating the React App**
- ``npx create-react-app configurator-frontend``

**Installing all necessary Packages**
- ``npm install react-redux``
  - the state container for react
- ``npm install @reduxjs/toolkit``
  - the toolkit to create write redux code more efficiently 

**Adding Material UI**
- ``npm install @mui/material``
- ``npm install @emotion/react``
- ``npm install @emotion/styled``
- ``npm install @mui/icons-material``


## Folder structure
- api
  - contains the scripts for api-communication with the backend
- assets
  - contains the images
- components
  - contains all the ui elements
- state
  - contains every component necessary for redux state management

## Redux Setup
**Create a store.js**
> state/store.js
```javascript
import { configureStore } from '@reduxjs/toolkit'
import productReducer from './product/productSlice'

export const store = configureStore({
    reducer: {
        product: productReducer
    }
})
```

**Provide the store globally**
> index.js
```javascript
import React from 'react'
import ReactDOM from 'react-dom'
import App from './App'
import { Provider } from 'react-redux'
import { store } from './app/store'

ReactDOM.render(
    <React.StrictMode>
        <Provider store={store}>
            <App />
        </Provider>
    </React.StrictMode>,
    document.getElementById('root')
)
```

**Creating a Slice**
- with redux toolkit, you define the state, reducers and actions all in one file
- easy to read and cleaner code

> state/product/productSlice.js
```javascript
import { createSlice } from '@reduxjs/toolkit'

const initialState = {
    products: [],
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const productSlice = createSlice({
    name: 'counter',
    initialState,
    reducers: {
        loadingStarted: (state) => {
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            state.status = 'succeeded'
            state.products = action.payload
        },
        loadingFailed: (state, action) => {
            state.status = 'failed'
            state.eroor = action.payload
        }
    }
})

// Action creators are generated for each case reducer function
export const { loadingStarted, loadingSucceeded, loadingFailed } = productSlice.actions

export default productSlice.reducer
```

**Using the state**
> App.js
```javascript
import { useSelector } from 'react-redux'

function App() {
    return (
        <div className="App">
            <h1>Test</h1>
        </div>
    )
}

export default App
```
