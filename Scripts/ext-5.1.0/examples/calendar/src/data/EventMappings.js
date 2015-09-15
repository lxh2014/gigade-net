//@define Ext.calendar.data.EventMappings
/**
 * @class Ext.calendar.data.EventMappings
 * A simple object that provides the field definitions for Event records so that they can
 * be easily overridden.
 *
 * To ensure the proper definition of Ext.calendar.data.EventModel the override should be
 * written like this:
 *
 *      Ext.define('MyApp.data.EventMappings', {
 *          override: 'Ext.calendar.data.EventMappings'
 *      },
 *      function () {
 *          // Update "this" (this === Ext.calendar.data.EventMappings)
 *      });
 */
Ext.ns('Ext.calendar.data'); //edit by wwei0216w
Ext.calendar.data.EventMappings = {
    EventId: {
        name: 'id',
        mapping: 'id',
        type: 'int'
    },
    CalendarId: {
        name: 'CalendarId',
        mapping: 'CalendarId',
        type: 'int'
    },
    Title: {
        name: 'Title',
        mapping: 'Title',
        type: 'string'
    },
    StartDate: {
        name: 'StartDate',
        mapping: 'StartDate',
        type: 'date'
    },
    StartDateStr: {
        name: 'StartDateStr',
        mapping: 'StartDateStr',
        type: 'string'
    },
    EndDate: {
        name: 'EndDate',
        mapping: 'EndDate',
        type: 'date'
    },
    EndDateStr: {
        name: 'EndDateStr',
        mapping: 'EndDateStr',
        type: 'string'
    },
    Location: {
        name: 'Location',
        mapping: 'Location',
        type: 'string'
    },
    Notes: {
        name: 'Notes',
        mapping: 'Notes',
        type: 'string'
    },
    Url: {
        name: 'Url',
        mapping: 'Url',
        type: 'string'
    },
    IsAllDay: {
        name: 'IsAllDay',
        mapping: 'IsAllDay',
        type: 'boolean'
    },
    Reminder: {
        name: 'Reminder',
        mapping: 'Reminder',
        type: 'string'
    },
    IsNew: {
        name: 'IsNew',
        mapping: 'IsNew',
        type: 'boolean'
    }
};

//Ext.calendar.data.EventMappings = {
//    EventId: {
//        name: 'EventId',
//        mapping: 'id',
//        type: 'int'
//    },
//    CalendarId: {
//        name: 'CalendarId',
//        mapping: 'cid',
//        type: 'int'
//    },
//    Title: {
//        name: 'Title',
//        mapping: 'title',
//        type: 'string'
//    },
//    StartDate: {
//        name: 'StartDate',
//        mapping: 'start',
//        type: 'date',
//        dateFormat: 'c'
//    },
//    EndDate: {
//        name: 'EndDate',
//        mapping: 'end',
//        type: 'date',
//        dateFormat: 'c'
//    },
//    Location: {
//        name: 'Location',
//        mapping: 'loc',
//        type: 'string'
//    },
//    Notes: {
//        name: 'Notes',
//        mapping: 'notes',
//        type: 'string'
//    },
//    Url: {
//        name: 'Url',
//        mapping: 'url',
//        type: 'string'
//    },
//    IsAllDay: {
//        name: 'IsAllDay',
//        mapping: 'ad',
//        type: 'boolean'
//    },
//    Reminder: {
//        name: 'Reminder',
//        mapping: 'rem',
//        type: 'string'
//    },
//    IsNew: {
//        name: 'IsNew',
//        mapping: 'n',
//        type: 'boolean'
//    }
//};
