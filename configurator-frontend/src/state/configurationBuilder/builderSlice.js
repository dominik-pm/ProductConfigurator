import { createSlice } from '@reduxjs/toolkit'
import { postConfiguration } from '../../api/configurationAPI'
import { getDoesGroupdExist, getDoesOptionExist, getDoesSectionExist, selectConfiguration } from './builderSelectors'


const initialState = {
    configuration: {
        name: '',
        description: '',
        images: [],
        options: [
            {
                id: 'ALLOY19',
                name: '19 inch Alloy',
                description: 'description',
            },
            {
                id: 'STEEL16',
                name: '16 inch Steel',
                description: 'description',
            }
        ],
        optionSections: [
            {
                id: 'EXTERIOR',
                name: 'Exterior',
                optionGroupIds: [
                    'WHEELS'
                ]
            }
        ],
        optionGroups: [
            {
                id: 'WHEELS',
                name: 'Wheels',
                description: 'round objects for diriving',
                optionIds: [
                    'ALLOY19', 'STEEL16'
                ],
                required: true
            }
        ],
        rules: {
            basePrice: 0,
            // defaultOptions: [/*BLUE*/],
            defaultModel: '',
            models: [
                {
                    modelName: 'Sport',
                    options: ['ALLOY19'],
                    description: 'description, description, description, description, description, description, description, description, description, description, description, description, description, description, description, description,'
                },
                {
                    modelName: 'Basic',
                    options: ['STEEL16'],
                    description: 'description, description, description, description, description, description, description, description, '
                }
            ],
            replacementGroups: {
                // COLOR_GROUP: [
                //     'BLUE'
                // ]
            },
            groupRequirements: {
                // PANORAMATYPE_GROUP: ['PANORAMA_GROUP']
            },
            requirements: {
                // D150: ['DIESEL']
            },
            incompatibilites: {
                // PANORAMAROOF: ['PETROL']
            },
            priceList: {
                // D150: 8000
            }
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
            const { groupId, name, description } = action.payload

            state.configuration.options.push({
                id: name,
                name: name,
                description: description
            })

            const group = state.configuration.optionGroups.find(g => g.id === groupId)
            if (group) group.optionIds.push(name)
        },
        addModel: (state, action) => {
            const { modelName, options, description } = action.payload

            state.configuration.rules.models.push({
                modelName, 
                options, 
                description
            })
        },
        removeModel: (state, action) => {
            state.configuration.rules.models = state.configuration.rules.models.filter(model => model.modelName !== action.payload)
        },
        setDefaultModel: (state, action) => {
            state.configuration.rules.defaultModel = action.payload
        },
        setModelOptions: (state, action) => {
            const { modelName, options } = action.payload
            
            const model = state.configuration.rules.models.find(m => m.modelName === modelName)
            if (model) model.options = options
        },
        setBasePrice: (state, action) => {
            state.configuration.rules.basePrice = action.payload
        },
        setDescription: (state, action) => {
            state.configuration.description = action.payload
        },
        resetBuild: (state, action) => {
            state.configuration = {}
        },
        loadingStarted: (state) => {
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            state.status = 'succeeded'
        },
        loadingFailed: (state, action) => {
            state.status = 'failed'
            state.error = action.payload
        },
        loadingHandled: (state, action) => {
            state.status = 'idle'
            state.error = null
        }
    },
    extraReducers: (builder) => {
        // builder.addCase(actionFromOtherSlice, (state, action) => {
        //     state.status = 'succeeded'
        // })
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

export const createOption = (groupId, name, description) => (dispatch, getState) => {
    // check if option doesn't already exist
    const optionExists = getDoesOptionExist(getState(), name)
    if (optionExists) {
        return false
    }

    dispatch(addOption({groupId, name, description}))
    return true
}

export const createModel = (modelName, options, description) => (dispatch) => {
    dispatch(addModel({
        modelName,
        options,
        description
    }))
}
export const createDefaultModel = (modelName) => (dispatch) => {
    dispatch(setDefaultModel(modelName))
}
export const changeModelOptions = (modelName, options) => (dispatch) => {
    dispatch(setModelOptions({
        modelName,
        options
    }))
}

export const finishConfigurationBuild = (name = '') => async (dispatch, getState) => {
    dispatch(loadingStarted())

    let configuration = selectConfiguration(getState())
    
    if (name) {
        configuration.name = name
    }

    postConfiguration(configuration)
    .then(res => {
        dispatch(loadingSucceeded(res))
    })
    .catch(error => {
        dispatch(loadingFailed(error))
    })
}



// Action creators are generated for each case reducer function
export const { addSection, addOptionGroup, addOption, addModel, removeModel, setDefaultModel, setModelOptions, setBasePrice, setDescription, resetBuild, loadingStarted, loadingSucceeded, loadingFailed, loadingHandled } = builderSlice.actions

export default builderSlice.reducer