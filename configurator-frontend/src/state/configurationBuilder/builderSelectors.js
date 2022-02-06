import { createSelector } from '@reduxjs/toolkit'

export const selectConfiguration = (state) =>                   state.builder.configuration
export const selectBuilderStatus = (state) =>                   state.builder.status
export const selectBuilderError = (state) =>                    state.builder.error

export const selectBuilderSections = (state) =>                 state.builder.configuration.optionSections
export const selectBuilderGroups = (state) =>                   state.builder.configuration.optionGroups
export const selectBuilderOptions = (state) =>                  state.builder.configuration.options
export const selectBuilderModels = (state) =>                   state.builder.configuration.rules.models
export const selectBuilderDefaultModel = (state) =>             state.builder.configuration.rules.defaultModel
export const selectBuilderOptionRequirements = (state) =>       state.builder.configuration.rules.requirements
export const selectBuilderOptionIncompatibilities = (state) =>  state.builder.configuration.rules.incompatibilities
export const selectBuilderGroupRequirements = (state) =>        state.builder.configuration.rules.groupRequirements || []

const selectName = (state, name) =>                     name

export const getBuilderOptionById = createSelector([selectName, selectBuilderOptions], (optionId, options) => {
    const option = options.find(o => o.id === optionId)
    return option ? option : null
})
export const getBuilderSectionById = createSelector([selectName, selectBuilderSections], (sectionId, sections) => {
    const section = sections.find(s => s.id === sectionId)
    return section ? section : null
})
export const getBuilderGroupById = createSelector([selectName, selectBuilderGroups], (groupId, groups) => {
    const group = groups.find(g => g.id === groupId)
    return group ? group : null
})
export const getBuilderOptionRequirementsByOptionId = createSelector([selectName, selectBuilderOptionRequirements], (optionId, requirements) => {
    const optionReq = requirements[optionId]
    return optionReq ? optionReq : []
})
export const getBuilderOptionIncompatibilitiesByOptionId = createSelector([selectName, selectBuilderOptionIncompatibilities], (optionId, incompatibilities) => {
    const optionIncomp = incompatibilities[optionId]
    return optionIncomp ? optionIncomp : []
})
export const getBuilderGroupRequirementsByGroupId = createSelector([selectName, selectBuilderGroupRequirements], (groupId, requirements) => {
    const groupReq = requirements[groupId]
    return groupReq ? groupReq : []
})

// export const getBuilderModelOptions = createSelector([selectName, selectBuilderModels, selectBuilderOptions], (modelName, models, options) => {
//     const model = models.find(m => m.modelName === modelName)
//     return model.options
// })

export const getDoesSectionExist = createSelector([selectBuilderSections, selectName], (sections, sectionName) => {
    return sections.find(s => s.id.toUpperCase() === sectionName.toUpperCase()) ? true : false
})
export const getDoesGroupdExist = createSelector([selectBuilderGroups, selectName], (groups, groupName) => {
    return groups.find(g => g.id.toUpperCase() === groupName.toUpperCase()) ? true : false
})
export const getDoesOptionExist = createSelector([selectBuilderOptions, selectName], (options, optionName) => {
    return options.find(o => o.id.toUpperCase() === optionName.toUpperCase()) ? true : false
})


export const extractOptionsFromBuilderGroup = (group) =>            group.optionIds || []
export const extractGroupsFromBuilderSection = (section) =>         section.optionGroupIds || []
export const extractModelNameFromBuilderModel = (model) =>          model.name || ''
export const extractModelOptionsFromBuilderModel = (model) =>       model.options || []