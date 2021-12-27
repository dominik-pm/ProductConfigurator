# Product Configurator Frontend

## Dependency Setup
**Creating the React App**
- ``npx create-react-app configurator-frontend``

**Installing all necessary Packages**
- ``npm install react-redux``
  - the state container for react
- ``npm install @reduxjs/toolkit``
  - the toolkit to create write redux code more efficiently 
- ``npm install react-router-dom@6``
  - react router version 6 for browser paths

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

***

## Redux Setup
**Create a store.js**
> state/store.js
```javascript
import { configureStore } from '@reduxjs/toolkit'
import productReducer from './product/productSlice'
import configurationReducer from './configuration/configurationSlice'

export const store = configureStore({
    reducer: {
        product: productReducer,
        configuration: configurationReducer,
        language: 
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

*Product State*
> state/product/productSlice.js
```javascript
import { createSlice } from '@reduxjs/toolkit'
import { fetchAll } from '../../api/productsAPI'

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
            console.log('fetching products...')
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            console.log('products loaded:', action.payload)
            state.status = 'succeeded'
            state.products = action.payload
        },
        loadingFailed: (state, action) => {
            console.log('products loading failed:', action.payload)
            state.status = 'failed'
            state.error = action.payload
        }
    }
})

export const fetchProducts = () => async (dispatch) => {
    dispatch(loadingStarted())

    fetchAll()
    .then(res => {
        dispatch(loadingSucceeded(res.products))
    })
    .catch(err => {
        dispatch(loadingFailed(err))
    })
}

// Action creators are generated for each case reducer function
export const { loadingStarted, loadingSucceeded, loadingFailed } = productSlice.actions

export default productSlice.reducer
```

*Language State*
> state/language/languageSlice.js
```javascript
import { createSlice } from '@reduxjs/toolkit'

const defaultLang = 'EN'
const localStorageLang = localStorage.getItem('language')

const initialState = {
    language: localStorageLang ? localStorageLang : defaultLang
    // status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    // error: null
}

export const languageSlice = createSlice({
    name: 'language',
    initialState,
    reducers: {
        changedLanguage: (state, action) => {
            state.language = action.payload
        }
    }
})

export const setLanguage = (lang) => async (dispatch) => {
    localStorage.setItem('language', lang)
    dispatch(changedLanguage(lang))
}
```

*Configuration State*
> state/configuration/configurationSlice.js
```javascript
import { createSlice } from '@reduxjs/toolkit'
import { fetchId } from '../../api/configurationAPI'

const initialState = {
    configuration: {},
    selectedOptions: [],
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const configurationSlice = createSlice({
    name: 'configuration',
    initialState,
    reducers: {
        selectOption: (state, action) => {
            if (!state.selectedOptions.includes(action.payload)) {
                // console.log('selecting option', action.payload)
                state.selectedOptions.push(action.payload)
            }
        },
        deselectOption: (state, action) => {
            // console.log('deselecting option', action.payload)
            state.selectedOptions = state.selectedOptions.filter(optionId => optionId !== action.payload)
        },
        loadingStarted: (state) => {
            console.log('fetching products...')
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            console.log('configuration loaded:', action.payload)
            state.status = 'succeeded'
            state.configuration = action.payload
            state.selectedOptions = action.payload.rules.defaultOptions
        },
        loadingFailed: (state, action) => {
            console.log('configuration loading failed:', action.payload)
            state.status = 'failed'
            state.error = action.payload
            state.configuration = {}
            state.selectedOptions = []
        }
    }
})


export const fetchConfiguration = (id) => async (dispatch) => {
    dispatch(loadingStarted())

    fetchId(id)
    .then(res => {
        dispatch(loadingSucceeded(res.configuration))
    })
    .catch(error => {
        dispatch(loadingFailed(error))
    })
}

// Action creators are generated for each case reducer function
export const { loadingStarted, loadingSucceeded, loadingFailed } = configurationSlice.actions

export default configurationSlice.reducer
```


**Using the state**
> components/product/ProductView.js
```javascript
import React, { useEffect } from 'react'
import Product from './Product'
import { connect } from 'react-redux'
import { Typography } from '@mui/material'
import { fetchProducts } from '../../state/product/productSlice'
import { selectProductError, selectProducts, selectProductStatus } from '../../state/product/productSelector'
import { selectLanguage } from '../../state/language/languageSelectors'
import { translate } from '../../lang'
import './ProductView.css'

function ProductView({ products, status, error, fetchProducts, language }) {

    useEffect(() => {
        if (isEmpty) {
            console.log('calling to fetch products...')
            fetchProducts() // dont need to dispatch any more (-> mapDispatchToProps)
        }
    }, [fetchProducts, isEmpty])


    function render() {
        switch (status) {
            case 'loading':
                return renderLoadingProducts()
            case 'succeeded':
                return (isEmpty ? renderEmptyProducts() : renderProducts())
            case 'failed':
                return renderApiFailed(error)
            default:
                return renderLoadingProducts()
        }
    }

    function renderLoadingProducts() {
        return (
            <Typography variant="h2">{translate('loadingProducts', language)}...</Typography>
        )
    }

    function renderEmptyProducts() {
        return (
            <Typography variant="h2">{translate('noProductsFound', language)}</Typography>
        )
    }
    function renderProducts() {
        return (
            <div className="ProductContainer">
                {
                    products.map(product => (
                        <Product key={product.id} product={product}></Product>
                    ))
                }
            </div>
        )
    }

    function renderApiFailed(errorMessage) {
        return (
            <div>
                <Typography variant="h1">{translate('failedToLoadProducts', language)}</Typography>
                <Typography variant="body1">{translate(errorMessage, language)}</Typography>
            </div>
        )
    }
    
    return (
        redner()
    )
}

// easier (also more generic) handling with the redux state
const mapStateToProps = (state) => ({
    products: selectProducts(state),
    status: selectProductStatus(state),
    error: selectProductError(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    fetchProducts
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(ProductView)
```

***

## Routing
create routes in App.js
> App.js
```javascript
function App() {
    return (
        <div className="App">
            <Router>
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
    )
}
```

navigate to a route
> components/products/Product.js
```javascript
import { useNavigate } from 'react-router'

export default function Product({product}) {
    const navigate = useNavigate()

    function handleClick(id) {
        navigate(`/configuration/${id}`)
    }

}
```


***

## API
**Products API**
- contains the needed request to fetch the products from the backend
> api/productsAPI.js
```javascript
export const fetchAll = () => {    
    return fetchApiTest()
}

// A mock function to mimic making an async request for data
function fetchApiTest(amount = products.length) {
    return new Promise((resolve, reject) =>
        // setTimeout(() => reject('AUTHENTICATION FAILED'), 500)
        setTimeout(() => resolve({
            error: null,
            products
        }), 500)
    )
}

const products = [
    {
        id: 0,
        name: 'Car',
        description: 'its a car, what else did you think?',
        image: '1.jpg'
    },
    ...
]
```

**Configuration API**
> api/configurationAPI.js
```javascript
export const fetchId = (productId) => {
    return fetchApiTest(productId)
}

// A mock api request function to mimic making an async request for data
const testDelay = 0;
function fetchApiTest(configId) {
    return new Promise((resolve, reject) =>
        setTimeout(() => {
            
            const conf = configurations.find(c => c.id === configId)

            if (!conf) {
                reject('no configuration found')
            }
            if (!conf.options || !conf.optionGroups || !conf.optionSections || !conf.rules) {
                reject('Configuration invalid!')
            }

            resolve(conf)

        }, testDelay)
    )
}

const configurations = [
    {
        id: 0,
        name: 'Car',
        description: 'its a car, to drive from A to B',
        image: '1.jpg',
        options: [
            {
                id: 'BLUE',
                name: 'Blue',
                description: 'A blue color',
                image: ''
            },
            ...
        ],
        optionSections: [
            {
                id: 'EXTERIOR',
                name: 'Exterior',
                optionGroupIds: [
                    'COLOR_GROUP'
                ]
            },
            ...
        ],
        optionGroups: [
            {
                id: 'COLOR_GROUP',
                name: 'Color',
                description: 'the exterior color of the car',
                optionIds: [
                    'BLUE', 'YELLOW', 'GREEN'
                ]
            },
            ...
        ],
        rules: {
            basePrice: 10000,
            defaultOptions: ['BLUE', 'DIESEL', 'D150'],
            replacementGroups: {
                COLOR_GROUP: [
                    'BLUE', 'YELLOW', 'GREEN'
                ],
                ...
            },
            requirements: {
                D150: ['DIESEL'],
                ...
            },
            incompatibilites: {
                PANORAMAROOF: ['PETROL'],
                ...
            },
            priceList: {
                D150: 8000,
                ...
            }
        }
```

***

## Components
### Products
**ProductView**
- is the container for all the available products
- based on the product reducer state ('loading', 'succeeded', 'failed'), it renders different components

**Product**
- the product component defines how a listed product looks

### Configuration
**ConfigurationView**
- manages the loading of the configuration
- displays `Configurator` when the configuration has loaded

**Configurator**
- is the layout of the configurator
- holds the `Summary`, `OptionsTabs` and a Header

**Summary**
- displays the current selected options and the total price of the product

**OptionTabs**
- has the tab panels for the different sections that contain all `OptionGroup`

**OptionGroup**
- displays the details of the group
- shows every available `Option`

**Option**
- shows the details of this option (name, description, price, ...)
- can be clicked on in order to select the option

