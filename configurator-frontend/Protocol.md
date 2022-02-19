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
- ``npm install react-slideshow-image``
  - to create an imageview as a slideshow

**Adding Material UI**
- ``npm install @mui/material``
- ``npm install @emotion/react``
- ``npm install @emotion/styled``
- ``npm install @mui/icons-material``

**SweetAlert2 - Alerts**
- ``npm install sweetalert2``


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

const authToken = localStorage.jwtToken
if (authToken) {
    setAuthorizationToken(authToken)
    store.dispatch(setCurrentUser(jwt.decode(authToken)))
}

const localStorageLang = localStorage.language
if (localStorageLang) {
    store.dispatch(setLanguage(localStorageLang))
}
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
import { setAcceptLanguage } from '../../api/general'
import { defaultLang, languageNames } from '../../lang'

const initialState = {
    language: defaultLang
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
    if (!Object.values(languageNames).includes(lang)) {
        lang = defaultLang
    }
    
    localStorage.setItem('language', lang)
    setAcceptLanguage(lang)
    dispatch(changedLanguage(lang))
}

// Action creators are generated for each case reducer function
export const { changedLanguage } = languageSlice.actions

export default languageSlice.reducer
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
    user: {},
    savedConfigurations: [],
    orderedConfigurations: [],
    allOrderedConfigurations: []
}

export const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        setCurrentUser: (state, action) => {
            state.user = action.payload
            
            if (Object.keys(action.payload).length > 0) {
                // user set
                state.isAuthenticated = Object.keys(action.payload).length > 0
            } else {
                // user removed
                state.isAuthenticated = false
                state.savedConfigurations = []
                state.orderedConfigurations = []
                state.allOrderedConfigurations = []
            }
        },
        setSavedConfigurations: (state, action) => {
            state.savedConfigurations = action.payload
        },
        setOrderedConfigurations: (state, action) => {
            state.orderedConfigurations = action.payload
        },
        setAllOrderedConfigurations: (state, action) => {
            state.allOrderedConfigurations = action.payload
        }
    }
})

export const getSavedConfigurations = () => async (dispatch) => {
    fetchSavedConfigurations()
    .then(configurations => {
        let saved = configurations.filter(config => config.status === 'saved')
        let ordered = configurations.filter(config => config.status === 'ordered')

        if (saved.length > 0) dispatch(setSavedConfigurations(saved))
        if (ordered.length > 0) dispatch(setOrderedConfigurations(ordered))
    })
    .catch(err => { console.log(err) })
}
export const getAllOrderedConfigurations= () => async (dispatch) => {
    fetchAllOrderedConfigurations()
    .then(configurations => {
        dispatch(setAllOrderedConfigurations(configurations))
    })
    .catch(err => { console.log(err) })
}

export const register = (username, password, email) => async (dispatch) => {
    requestRegister(username, password, email).then(res => {
        // display registered notification
    })
    .catch(err => { console.log(err) })
}

export const login = (username, password) => async (dispatch) => {
    requestLogin(username, password).then(res => {
        const { token, user } = res

        localStorage.setItem('jwtToken', token)
        setAuthorizationToken(token)
        dispatch(setCurrentUser(user))
    })
    .catch(err => { console.log(err) })
}

export const logout = () => (dispatch) => {
    localStorage.removeItem('jwtToken')
    setAuthorizationToken(false)
    dispatch(setCurrentUser({}))
}

// Action creators are generated for each case reducer function
export const { setCurrentUser, setSavedConfigurations, setOrderedConfigurations, setAllOrderedConfigurations } = userSlice.actions

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

*Alert State*
> state/alert/alertSlice.js
```javascript
import { createSlice } from '@reduxjs/toolkit'

export const alertStatus = {
    CLOSED: 'closed',
    OPEN: 'open'
}

export const alertTypes = {
    ERROR: 'error',
    WARNING: 'warning',
    INFO: 'info',
    SUCCESS: 'success'
}

const initialState = {
    status: alertStatus.CLOSED,
    alerts: [ // can be multiple -> when 1. closed, 2nd pops up
        // {message: '', type: alertTypes.INFO}
    ]
}

export const alertSlice = createSlice({
    name: 'alert',
    initialState,
    reducers: {
        addAlert: (state, action) => {
            state.status = alertStatus.OPEN
            state.alerts.push(action.payload)
        },
        closeAlert: (state) => {
            state.alerts.shift()

            // set the status to closed, when all alerts are gone
            if (state.alerts.length === 0) {
                state.status = alertStatus.CLOSED
            }
        }
    }
})

export const openAlert = (message, type) => async (dispatch) => {
    dispatch(addAlert({ message, type }))
}

// Action creators are generated for each case reducer function
export const { addAlert, closeAlert } = alertSlice.actions

export default alertSlice.reducer
```

*Builder State*
> state/builder/builderSlice.js
```javascript
const initialState = {
    configuration: {
        name: '',
        description: '',
        images: [],
        options: [],
        optionSections: [],
        optionGroups: [],
        rules: {
            basePrice: 0,
            defaultOptions: [],
            replacementGroups: {},
            groupRequirements: {},
            requirements: {},
            incompatibilites: {},
            priceList: {}
        }
    },
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const builderSlice = createSlice({
    name: 'builder',
    initialState,
    reducers: {
        addSection: (state, action) => {
            state.configuration.optionSections.push({
                id: action.payload,
                name: action.payload,
                optionGroupIds: []
            })
        },
        addOptionGroup: (state, action) => {
            const { sectionId, name, description, required } = action.payload

            state.configuration.optionGroups.push({
                id: name,
                name: name,
                description: description,
                required: required,
                optionIds: []
            })

            const section = state.configuration.optionSections.find(s => s.id === sectionId)
            if (section) section.optionGroupIds.push(name)
        },
        addOption: (state, action) => {
            console.log('adding option...')
            // TODO: optionname_optiongroup_configid
        },
        resetBuild: (state, action) => {
            state.configuration = {}
        }
    }
})

export const createSection = (sectionName) => (dispatch, getState) => {
    // check if section doesn't already exist
    const sectionExists = getDoesSectionExist(getState(), sectionName)
    if (sectionExists) {
        return false
    }

    dispatch(addSection(sectionName))
    return true
}

export const createGroup = (sectionId, name, description, required) => (dispatch, getState) => {
    // check if section doesn't already exist
    const groupExists = getDoesGroupdExist(getState(), name)
    if (groupExists) {
        return false
    }

    dispatch(addOptionGroup({sectionId, name, description, required}))
    return true
}

export const finishConfigurationBuild = () => async (dispatch, getState) => {
    const configuration = selectConfiguration(getState())

    postConfiguration(configuration)
    .then(res => {})
    .catch(error => {})
}



// Action creators are generated for each case reducer function
export const { addSection, addOptionGroup, addOption, resetBuild } = builderSlice.actions

export default builderSlice.reducer
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
                        <Product key={product.configId} product={product}></Product>
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

## Multi Language
- translation files (json files with translation keys and values for the translation of the specific language)
> lang/en.json
```json
{
    "cancel": "cancel",
    "confirm": "confirm",
    "configureYour": "Configure your",
    "price": "price",
    "saveConfiguration": "Save Configuration",
    "configurationName": "Configuration Name",
    "resetConfiguration": "Reset Configuration",
    "finishConfiguration": "Order Configuration",
    "resetConfigurationPrompt": "Do you really want to reset your current configuration?",
    "Login": "Login",
    "Register": "Register",
    "submit": "submit"
}
```

- translation script (to translate from a key to a given language)
> lang/index.js
```javascript
import en from './en.json'
import de from './de.json'
import fr from './fr.json'

export const languageNames = {
    EN: 'en',
    DE: 'de',
    FR: 'fr'
}
export const defaultLang = languageNames.EN

const languages = {
    'en': en,
    'de': de,
    'fr': fr
}

export const translate = (key, language) => {
    let langData = languages[language]

    if (!langData) {
        console.log(`Trying to translate ${key} to a language that doesn't exist: '${language}'`)
        return
    }

    const translation = langData[key]

    if (!translation) {
        // if there is a translation in the default language, at least return that
        if (languages[defaultLang][key]) {
            return languages[defaultLang][key]
        }

        // there is no translation for this key (not even in the default language) -> just return the key
        return key
    }

    return translation
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


### Configurator Logic
**Groups**
- a group can be a replacement group
  - only one option in this group can be selected
- a group can be required
  - at least one option in this group has to be selected, or the configuration is invalid
- a group can have other group requirements
  - this group can only be selected if each required group is select as well
  - if this group is required but the required group (from the group requirements) is not required
    - the configuration is valid if none of the two groups are selected or both (if the required group is selected -> this group also has to be selected because it is required)
  - if both groups are required
    - the configuration is only in a valid state if both are selected (the required parent group is first selected then the group itself)

## Image Slideshow
**Using the slideshow**

> conponents/configuration/Configurator/Configurator.js
```javascript
import { Slide } from 'react-slideshow-image'
import 'react-slideshow-image/dist/styles.css'

<Slide easing="ease">
    {IMAGES.map((image, index) => (
        <div key={index} className="each-slide">
            <div style={{
                height: '60vw',
                maxHeight: '600px', 
                backgroundImage: `url(${getImageSource(image)})`,
                backgroundSize: 'cover',
                backgroundPosition: 'center'
            }}>
            </div>
        </div>
    ))}
</Slide>
```

**Helper function for getting the images**
> App.js
```javascript
export function getImageSource(image) {
    let imageSource = ''

    try {
        const src = require(`./assets/img/${image.replace('./', '')}`)
        imageSource = src.default
    } catch (err) {
        // image not found -> return default image
        const src = require(`./assets/img/notfound.jpg`)
        imageSource = src.default
    } finally {
        return imageSource
    }
}
```