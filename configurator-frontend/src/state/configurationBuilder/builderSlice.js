import { createSlice } from '@reduxjs/toolkit'
import { postConfiguration } from '../../api/configurationAPI'
import { getDoesGroupdExist, getDoesSectionExist, selectConfiguration } from './builderSelectors'


const initialState = {
    configuration: {
        name: '',
        description: '',
        images: [],
        options: [
            // {
            //     id: 'BLUE',
            //     name: 'Blue',
            //     description: 'A blue color',
            // }
        ],
        optionSections: [
            // {
            //     id: 'EXTERIOR',
            //     name: 'Exterior',
            //     optionGroupIds: [
            //         'COLOR_GROUP'
            //     ]
            // }
        ],
        optionGroups: [
            // {
            //     id: 'COLOR_GROUP',
            //     name: 'Color',
            //     description: 'the exterior color of the car',
            //     optionIds: [
            //         'BLUE', 'YELLOW', 'GREEN'
            //     ],
            //     required: true
            // }
        ],
        rules: {
            basePrice: 0,
            defaultOptions: [/*BLUE*/],
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
            console.log('adding option...')
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

export const finishConfigurationBuild = () => async (dispatch, getState) => {
    dispatch(loadingStarted())

    const configuration = selectConfiguration(getState())

    postConfiguration(configuration)
    .then(res => {
        dispatch(loadingSucceeded(res))
    })
    .catch(error => {
        dispatch(loadingFailed(error))
    })
}



// Action creators are generated for each case reducer function
export const { addSection, addOptionGroup, addOption, resetBuild, loadingStarted, loadingSucceeded, loadingFailed, loadingHandled } = builderSlice.actions

export default builderSlice.reducer