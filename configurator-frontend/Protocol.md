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
- lang
  - contains the translate function + the language files
- 

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

*Confirmation State*
> state/confirmation/confirmationSlice.js
```javascript
import { createSlice } from '@reduxjs/toolkit'
import { selectIsConfirmDialogOpen } from './confirmationSelectors'

const initialState = {
    open: false,
    message: '',
    content: {}
}

export const confirmationSlice = createSlice({
    name: 'confirmation',
    initialState,
    reducers: {
        show: (state, action) => {
            // console.log('Opened confirmation dialog: ' + message)

            const { message, content } = action.payload

            state.open = true
            state.message = message
            state.content = content
        },
        close: (state, action) => {
            // console.log('Closed confirmation dialog')
            state.open = false
            state.message = ''
            state.content = {}
        }
    }
})

let onConfirm = null

export const dialogOpen = (message, content, onConfirmCallback) => (dispatch, getState) => {
    const isOpen = selectIsConfirmDialogOpen(getState())
    if (isOpen) {
        console.log('Confirmation Dialog is already open!')
        return
    }

    onConfirm = onConfirmCallback
    
    dispatch(show({message, content}))
}

export const dialogConfirm = () => (dispatch, getState) => {
    const isOpen = selectIsConfirmDialogOpen(getState())
    if (!isOpen) {
        console.log('Confirmation Dialog is closed (can not confirm)!')
        return
    }

    dispatch(close())
    if (onConfirm) {
        onConfirm()
    } else {
        console.log('no confirmation callback')
    }
    onConfirm = null
}

export const dialogCancel = () => (dispatch) => {
    dispatch(close())
    onConfirm = null
}

// Action creators are generated for each case reducer function
export const { show, close } = confirmationSlice.actions

export default confirmationSlice.reducer
``` 

*User State*
> state/user/userSlice.js
```javascript
import { createSlice } from '@reduxjs/toolkit'
import { requestLogin, requestRegister, setAuthorizationToken } from '../../api/userAPI'

const initialState = {
    isAuthenticated: false,
    user: {}
}

export const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        setCurrentUser: (state, action) => {
            console.log('getting user:', action.payload)
            state.isAuthenticated = Object.keys(action.payload).length > 0
            state.user = action.payload
        }
    }
})

export const register = (username, password, email) => async (dispatch) => {
    requestRegister(username, password, email).then(res => {
        // display registered notification
    })
    .catch(err => {
        console.log(err)
    })
}

export const login = (username, password) => async (dispatch) => {
    
    requestLogin(username, password).then(res => {
        const { token, user } = res

        localStorage.setItem('jwtToken', token)
        setAuthorizationToken(token)
        dispatch(setCurrentUser(user))
    })
    .catch(err => {
        console.log(err)
    })
}

export const logout = () => (dispatch) => {
    localStorage.removeItem('jwtToken')
    setAuthorizationToken(false)
    dispatch(setCurrentUser({}))
}

// Action creators are generated for each case reducer function
export const { setCurrentUser } = userSlice.actions

export default userSlice.reducer
```

*Input Dialog State*
> state/inputDialog/inputDialogSlice.js
```javascript
import { createSlice } from '@reduxjs/toolkit'
import { selectInputDialogData, selectIsInputDialogOpen } from './inputDialogSelectors'

const initialState = {
    open: false,
    headerMessage: '',
    data: {}
}

export const inputDialogSlice = createSlice({
    name: 'inputDialog',
    initialState,
    reducers: {
        show: (state, action) => {
            const { headerMessage, data } = action.payload

            state.headerMessage = headerMessage
            state.data = data
            state.open = true
        },
        close: (state, action) => {
            state.open = false
            state.headerMessage = ''
            state.data = {}
        },
        setData: (state, action) => {
            state.data = action.payload
        }
    }
})


let onConfirm = null

export const inputDialogOpen = (headerMessage, data, onConfirmCallback) => (dispatch, getState) => {
    const isOpen = selectIsInputDialogOpen(getState())
    if (isOpen) {
        console.log('Input Dialog is already open!')
        return
    }

    onConfirm = onConfirmCallback
    
    dispatch(show({headerMessage, data}))
}
export const inputDialogConfirm = () => (dispatch, getState) => {
    const isOpen = selectIsInputDialogOpen(getState())
    if (!isOpen) {
        console.log('Input Dialog is closed (can not confirm)!')
        return
    }

    if (onConfirm) {
        const data = selectInputDialogData(getState())
        onConfirm(data)
    } else {
        console.log('no confirmation callback')
    }
    dispatch(close())
    onConfirm = null
}
export const inputDialogCancel = () => (dispatch) => {
    dispatch(close())
    onConfirm = null
}
export const inputDialogSetData = (data) => (dispatch) => {
    dispatch(setData(data))
}


// Action creators are generated for each case reducer function
export const { show, close, setData } = inputDialogSlice.actions

export default inputDialogSlice.reducer
```

**Creating Selectors**
- Input Selectors:
  - input selectors are great to get nested varaibles out of the state (so that the components does not have to know the structure of the state)
- Output Selectors:
  - with 'reselect' the selectors can be 'memoized', meaning that 'expensive' calculation are cached and only recalculated if the input parameters change
- Example:
> state/configurationSelectors.js
```javascript
// input selectors
const selectOptionId = (state, optionId) =>             optionId
export const selectOptions = state =>                   state.configuration.configuration.options
export const selectBasePrice = state =>                 state.configuration.configuration.rules.basePrice
export const selectDefaultOptions = state =>            state.configuration.configuration.rules.defaultOptions

// output selectors
export const getOption = createSelector([selectOptions, selectOptionId], (options, id) => {
    return options.find(o => o.id === id)
})
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
                <Header></Header>
                <ConfirmationOptionSelect></ConfirmationOptionSelect>
                <InputDialog></InputDialog>

                <Routes>

                    <Route exact path="/" element={
                        <ProductView></ProductView>
                    }>
                    </Route>

                    <Route exact path="/account" element={
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

### Dialog
**ConfirmationDialog**
- can be opened to confirm an action
- for example the user select item1 that is not compatible with item2, so the user is prompted to confirm to remove item2
> conponents/dialog/ConfirmationDialog.js
```javascript
function ConfirmationOptionSelect({ isOpen, message, cancel, confirm }) {
    function handleClose() {
        cancel()
    }

    function handleConfirm() {
        confirm()
    }
    
    return (
        <Dialog
            open={isOpen}
            onClose={handleClose}
            aria-labelledby="responsive-dialog-title"
        >
            <DialogTitle id="responsive-dialog-title">
                confirmation prompt
            </DialogTitle>

            <DialogContent dividers>
                 {message}
            </DialogContent>

            <DialogActions>
                <Button autoFocus onClick={handleClose}>
                    cancel
                </Button>
                <Button autoFocus onClick={handleConfirm}>
                    confirm
                </Button>
            </DialogActions>
        </Dialog>
    )
}
const mapStateToProps = (state) => ({
    message: selectConfirmDialogMessage(state),
    isOpen: selectIsConfirmDialogOpen(state)
})
const mapDispatchToProps = {
    cancel: useConfirmationDialog.cancel,
    confirm: useConfirmationDialog.confirm
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(ConfirmationOptionSelect)
```
- calling the dialog:
```javascript
import { useConfirmationDialog } from '../../state/confirmationDialog/confirmationSlice'

function Header({ open }) {

    return (
        <header>
            <Box>
                <AppBar position="static">
                    <Toolbar>
                        <Button variant="contained" onClick={() => 
                            open(
                                'Example Dialog Message',           // dialog message
                                {},                                 // custom dialog message
                                () => console.log('confirmed'))}    // callback function to execute after confirmation
                        >
                            Open Test Dialog
                        </Button>
                    </Toolbar>
                </AppBar>
            </Box>
        </header>
    )
}

const mapStateToProps = (state) => ({})
const mapDispatchToProps = {
    open: useConfirmationDialog.open
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Header)

```

**InputDialog**
- can be opened with a custom data object where an input field for every data object is generated
- when confirmed, the values of the input fields are written to the values of the returned data object
- using the dialog:
```javascript
import { inputDialogOpen } from '../../state/inputDialog/inputDialogSlice'
import { login } from '../../state/user/userSlice'

function LoginButton({ openInputDialog, login }) {

    function openLogInDialog() {
        const data = {
            username: {name: 'Username', value: ''},
            password: {name: 'Password', value: '', isPassword: true}
        }
        openInputDialog('Login', data, (data) => {
            login(data.username.value, data.username.password)
        })
    }

    return (
        <Button 
            variant="contained" 
            onClick={openLogInDialog}
            >
            Login
        </Button>
    )
}

const mapStateToProps = (state) => ({})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen,
    login: login
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(LoginButton)
```


