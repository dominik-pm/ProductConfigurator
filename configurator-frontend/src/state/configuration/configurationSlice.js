import { createSlice } from '@reduxjs/toolkit'
import { fetchId } from '../../api/configurationAPI'
import { useConfirmationDialog } from '../confirmationDialog/confirmationSlice'
import { getDependentOptionsDeselect, getDependentOptionsSelect, getIsOptionSelected, getOptionName, getOptionReplacementGroup } from './configurationSelectors'

const openDialog = useConfirmationDialog.open

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
    // // check if the option is in a replacement group 
    // const replacementGroup = getOptionReplacementGroup(getState(), id)
    // if (replacementGroup) {
    //     if (!getIsOptionSelected(getState(), id)) {
    //         dispatch(selectWithDependencies(id))
    //     }
        
    //     // -> deselect all other options
    //     for (const optionId of replacementGroup) {
    //         // dont dispatch the deselect action if its the option that gets selected 
    //         if (optionId === id) continue
    //         // dont dispatch if the option is already not selected
    //         if (!getIsOptionSelected(getState(), optionId)) continue
            
    //         dispatch(deselectWithDependencies(optionId))
    //     }

    //     return
    // }

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

    // get all options in the replacement group that will be disabled
    let replacementGroupOptionsToDeselect = []
    const replacementGroup = getOptionReplacementGroup(getState(), id)
    if (replacementGroup) {
        for (const optionId of replacementGroup) {
            // ignore if the option is the option that gets selected 
            if (optionId === id) continue

            // ignore if the option is already deselected selected
            if (!getIsOptionSelected(getState(), optionId)) continue
            
            replacementGroupOptionsToDeselect.push(optionId)
        }
    }


    // -> deeper options (not in the same group)
    // get all options that cant be used if this option is selected
    const incompatibleOptionsToDeselect = getDependentOptionsSelect(getState(), id)

    
    // for every option that has to be disabled, get all further dependencies 
    const optionsToDeselect = [...replacementGroupOptionsToDeselect, ...incompatibleOptionsToDeselect]

    let deeperOptionsToDeselect = []
    optionsToDeselect.forEach(option => {
        const dependencies = getDependenciesDeselect(getState(), option)
        if (dependencies) {
            // if the dependencies are not empty, add them to the list
            deeperOptionsToDeselect = [...deeperOptionsToDeselect, ...dependencies]
        }
    })
    deeperOptionsToDeselect = [...incompatibleOptionsToDeselect, ...deeperOptionsToDeselect]
    // <- deeper options (not in the same group)


    const allOptionsToDeselect = [...replacementGroupOptionsToDeselect, ...deeperOptionsToDeselect]

    if (!allOptionsToDeselect || allOptionsToDeselect.length === 0) {
        // no options to deselect if this option is selected -> just select the option
        dispatch(selectAndDeselectOptions([id], null))
        return
    }
    
    // console.log(deeperOptionsToDeselect)
    if (deeperOptionsToDeselect.length === 0) {
        // no confirmation prompt required (deselected options are just from replacementgroup)
        dispatch(selectAndDeselectOptions([id], allOptionsToDeselect))
        return
    }
    
    // confirmation prompt for incompatibilities with the selected option
    const incompatibleOptionNames = deeperOptionsToDeselect.map(dependentId => getOptionName(getState(), dependentId))
    const selectedOptionName = getOptionName(getState(), id)

    const confirmMessage = `By selecting ${selectedOptionName} you will deselect ${incompatibleOptionNames.join(', ')}`
    dispatch(openDialog(confirmMessage, {selected: id, deselected: null, optionsToSelect: [], optionsToRemove: deeperOptionsToDeselect}, () => {
        // console.log('Confirmed')
        // select the option and deselect all incompatible options
        dispatch(selectAndDeselectOptions([id], allOptionsToDeselect))
    }))
}
// select an option and look out for dependencies (deselect all others that depend on it)
const deselectWithDependencies = (id) => (dispatch, getState) => {

    // get all options that can only be used if this option was selected
    const dependentOptions = getDependenciesDeselect(getState(), id)
    
    // if there are no dependent options, just deselect this option
    if (!dependentOptions || dependentOptions.length === 0) {
        dispatch(selectAndDeselectOptions(null, [id]))
        return
    }

    // console.log('all deselect dependencies: ')
    // console.log(dependentOptions)

    // confirmation Prompt for deselecting 'deeper dependentOptions'
    const dependentOptionNames = dependentOptions.map(dependentId => getOptionName(getState(), dependentId))
    const deselectedOptionName = getOptionName(getState(), id)

    const confirmMessage = `By deselecting ${deselectedOptionName} you will also deselect ${dependentOptionNames.join(', ')}`
    dispatch(openDialog(confirmMessage, {selected: null, deselected: id, optionsToSelect: [], optionsToRemove: dependentOptions}, () => {
        // console.log('Confirmed')
        // deselect the option and all dependent options
        dispatch(selectAndDeselectOptions(null, [id].concat(dependentOptions)))
    }))

    // deselect this option and all the options that depend on it
    // dispatch(selectAndDeselectOptions(null, [id].concat(dependentOptions)))
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
export const { selectOption, deselectOption, loadingStarted, loadingSucceeded, loadingFailed } = configurationSlice.actions

export default configurationSlice.reducer