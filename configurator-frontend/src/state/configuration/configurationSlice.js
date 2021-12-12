import { createSlice } from '@reduxjs/toolkit'
import { fetchId } from '../../api/configurationAPI'
import { getDependentOptionsDeselect, getDependentOptionsSelect, getIsOptionSelected, getOptionReplacementGroup } from './configurationSelectors'

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
            console.log('fetching configuration...')
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
    },
    extraReducers: (builder) => {
        // builder.addCase(actionFromOtherSlice, (state, action) => {
        //     state.status = 'succeeded'
        // })
    }
})

// handle the click on an option
export const clickedOption = (id) => (dispatch, getState) => {
    // check if the option is in a replacement group 
    const replacementGroup = getOptionReplacementGroup(getState(), id)
    if (replacementGroup) {
        // -> select the clicked option
        if (!getIsOptionSelected(getState(), id)) {
            dispatch(selectWithDependencies(id))
        }
        
        // -> deselect all other options
        for (const optionId of replacementGroup) {
            // dont dispatch the deselect action if its the option that gets selected 
            if (optionId === id) continue
            // dont dispatch if the option is already not selected
            if (!getIsOptionSelected(getState(), optionId)) continue
            
            dispatch(deselectWithDependencies(optionId))
        }

        return
    }


    if (getIsOptionSelected(getState(), id)) {
        // option is selected
        dispatch(deselectWithDependencies(id))
    } else {
        // option is not selected
        dispatch(selectWithDependencies(id))
    }

}
// select an option and look out for dependencies (deselect all that are incompatible with it)
const selectWithDependencies = (id) => (dispatch, getState) => {
    // get all options that cant be used if this option is selected
    const optionsToDeselect = getDependentOptionsSelect(getState(), id)

    // for every option that has to be disabled, get all further dependencies 
    let allOptionsToDeselect = [...optionsToDeselect]
    optionsToDeselect.forEach(option => {
        const x = getDependenciesDeselect(getState(), option)
        if (x) {
            // if the dependencies are not empty, add them to the list
            allOptionsToDeselect = [...allOptionsToDeselect, ...x]
        }
    })

    // console.log('all select dependencies: ')
    // console.log(allOptionsToDeselect)
    
    // select the option, and deselect the others
    // TODO: open prompt here to ask something like 'by selecting x, you deselect y and z. CONFIRM?'
    dispatch(selectAndDeselectOptions([id], allOptionsToDeselect))
}
// select an option and look out for dependencies (deselect all others that depend on it)
const deselectWithDependencies = (id) => (dispatch, getState) => {
    // get all options that can only be used if this option was selected
    const dependentOptions = getDependenciesDeselect(getState(), id)
    
    // if there are no dependent options, just deselect this option
    if (!dependentOptions || dependentOptions.length === 0) {
        dispatch(selectAndDeselectOptions(null, [id]))
    } else {
        // console.log('all deselect dependencies: ')
        // console.log(dependentOptions)

        // deselect this option and all the options that depend on it
        dispatch(selectAndDeselectOptions(null, [id].concat(dependentOptions)))
    }
}
// dispatches the actions to select (1st parameter) and deselect (2nd parameter) all given options
export const selectAndDeselectOptions = (optionsToSelect, optionsToDeselect) => (dispatch) => {
    // if the options to select are valid, select all of them
    if (optionsToSelect && optionsToSelect.length >= 1) {
        optionsToSelect.forEach(option => {
            dispatch(selectOption(option))
        })
    }
    // if the options to deselect are valid, deselect all of them
    if (optionsToDeselect && optionsToDeselect.length >= 1) {
        optionsToDeselect.forEach(option => {
            dispatch(deselectOption(option))
        })
    }
}
// recursive function to get all options that depend on the deselected option 
const getDependenciesDeselect = (state, id) => {
    // deselect all options that cant be used if this option is deselected
    const dependentOptions = getDependentOptionsDeselect(state, id)

    if (dependentOptions.length === 0) return null

    let subDependentOptions = []

    dependentOptions.forEach(optionId => {
        // recursively get all dependencies from the dependent options
        const x = getDependenciesDeselect(state, optionId)
        if (x != null) {
            subDependentOptions = [...subDependentOptions, ...x]
        }
    })
    return [...dependentOptions, ...subDependentOptions]
}


export const fetchConfiguration = (id) => async (dispatch) => {
    dispatch(loadingStarted())

    fetchId(id)
    .then(res => {
        dispatch(loadingSucceeded(res))
    })
    .catch(error => {
        dispatch(loadingFailed(error))
    })
}

// Action creators are generated for each case reducer function
export const { clickOption, selectOption, deselectOption, loadingStarted, loadingSucceeded, loadingFailed } = configurationSlice.actions

export default configurationSlice.reducer