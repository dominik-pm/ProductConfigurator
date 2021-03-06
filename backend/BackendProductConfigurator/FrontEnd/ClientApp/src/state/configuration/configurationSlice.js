import { createSlice } from '@reduxjs/toolkit'
import { fetchId } from '../../api/configurationAPI'
import { readFromLocalStorage, writeToLocalStorage } from '../../App'
import { confirmDialogOpen } from '../confirmationDialog/confirmationSlice'
import { extractModelIdFromModel, extractModelOptionsFromModel, getDependentOptionsDeselect, getDependentOptionsSelect, getIsOptionSelected, getModelOptions, getOptionName, getOptionReplacementGroup, selectConfigurationId, selectDefaultModel, selectModels, selectSelectedOptions } from './configurationSelectors'

// const openDialog = useConfirmationDialog.open

const initialState = {
    configuration: {},
    selectedOptions: [],
    selectedModel: '',
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const configurationSlice = createSlice({
    name: 'configuration',
    initialState,
    reducers: {
        selectOption: (state, action) => {
            // console.log(first)
            if (!state.selectedOptions.includes(action.payload)) {
                console.log('selecting option', action.payload)
                state.selectedOptions.push(action.payload)
            }
        },
        deselectOption: (state, action) => {
            // console.log('deselecting option', action.payload)
            state.selectedOptions = state.selectedOptions.filter(optionId => optionId !== action.payload)
        },
        setSelectedOptions: (state, action) => {
            console.log('setting selected options')
            state.selectedOptions = action.payload
        },
        setSelectedModel: (state, action) => {
            // console.log('setting selected model: ' + action.payload)
            state.selectedModel = action.payload
        },
        reset: (state, action) => {
            console.log('reset active configuration')
            state.selectedOptions = action.payload
        },
        loadingStarted: (state) => {
            console.log('fetching configuration...')
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            console.log('configuration loaded:', action.payload)
            state.status = 'succeeded'
            state.configuration = action.payload
            state.selectedOptions = loadSelectedOptionsFromStorage(state.configuration.configId) || []
        },
        loadingFailed: (state, action) => {
            console.log('configuration loading failed:', action.payload)
            state.status = 'failed'
            state.error = action.payload.toString()
            state.configuration = {}
            state.selectedOptions = []
        }
    },
    extraReducers: (builder) => {
        // builder.addCase(selectOption, (state, action) => {
        //     checkModel()
        // })
    }
})

export const fetchConfiguration = (id) => async (dispatch, getState) => {
    dispatch(loadingStarted())

    fetchId(id)
    .then(res => {
        dispatch(setModel(selectDefaultModel(getState())))
        dispatch(loadingSucceeded(res))
        dispatch(checkModel())
    })
    .catch(error => {
        dispatch(loadingFailed(error))
    })
}

const containsAll = (arr1, arr2) => arr2.every(arr2Item => arr1.includes(arr2Item))
const sameMembers = (arr1, arr2) => arr1 && arr1.length > 0 && containsAll(arr1, arr2) && containsAll(arr2, arr1)
const checkModel = () => (dispatch, getState) => {
    const selectedOptions = selectSelectedOptions(getState())
    const models = selectModels(getState())
    
    if (selectedOptions.length === 0) {
        // no option is selected -> unset model
        dispatch(setModel(''))
        return
    }

    // check if the selected options are part of a model and then set this model as the current one
    for (const model of models) {
        const modelOptions = extractModelOptionsFromModel(model)
        if (modelOptions) {
            if (sameMembers(modelOptions, selectedOptions)) {
                dispatch(setModel(extractModelIdFromModel(model)))
                return
            }
        }
    }

    // no model matches the current configuration -> unset model
    dispatch(setModel(''))

}
export const setModel = (modelName = '') => (dispatch, getState) => {
    if (modelName) {
        const modelOptions = getModelOptions(getState(), modelName)
        dispatch(setSelectedOptions(modelOptions))
    }

    dispatch(setSelectedModel(modelName))
}

// save the currently active configuration to the local storage
export const saveActiveConfiguration = () => (dispatch, getState) => {
    const id = selectConfigurationId(getState())
    const options = selectSelectedOptions(getState())
    saveConfigurationToStorage(id, options)
}
// save the configuration data (id, options) to the local storage (or append to existing configurations)
export const saveConfigurationToStorage = (id, options) => {
    let configurations = loadConfigurationsFromStorage()

    let newConfiguration = configurations.find(c => c.id === id)
    if (newConfiguration) {
        // configuration is already saved -> update values
        newConfiguration.options = options
    } else {
        // add the new configuration to the array
        newConfiguration = { id, options }
        configurations.push(newConfiguration)
    }

    // save the updated configurations to the storage
    writeToLocalStorage(configurations, 'configurations')
}

// get the selected options for the specific configuration (id) from the storage
const loadSelectedOptionsFromStorage = (id) => {
    console.log('all stored configs:', loadConfigurationsFromStorage())
    console.log(id)
    const configuration = loadConfigurationsFromStorage().find(c => c.id === id)

    console.log('storage loaded config:', configuration)

    if (!configuration) return null

    if (!configuration.options) return null

    return configuration.options
}
const loadConfigurationsFromStorage = () => {
    let configurations = readFromLocalStorage('configurations')
    
    if (!configurations) return []

    return configurations
}

// reset the active confirguration
export const resetActiveConfiguration = () => (dispatch, getState) => {
    try {
        // const defaultOptions = selectDefaultOptions(getState())
        // dispatch(reset(defaultOptions))
        const defaultModel = selectDefaultModel(getState())
        dispatch(setModel(defaultModel))
    } catch {
        console.log('Can not reset -> no configuration found')
    }
}



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
    dispatch(confirmDialogOpen(confirmMessage, {selected: id, deselected: null, optionsToSelect: [], optionsToRemove: deeperOptionsToDeselect}, null, () => {
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
    dispatch(confirmDialogOpen(confirmMessage, {selected: null, deselected: id, optionsToSelect: [], optionsToRemove: dependentOptions}, null, () => {
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

    // after adjusting the current selection -> save the configuration
    dispatch(saveActiveConfiguration())

    // check if the model changes based on the different selected options
    dispatch(checkModel())

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

// Action creators are generated for each case reducer function
export const { selectOption, deselectOption, setSelectedOptions, setSelectedModel, reset, loadingStarted, loadingSucceeded, loadingFailed } = configurationSlice.actions

export default configurationSlice.reducer