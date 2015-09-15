Ext.define('Ext.calendar.data.Calendars', {
    statics: {
        getData: function(){
            return {
                "calendars":[{
                    "id":    1,
                    "title": "工作日"
                },{
                    "id":    2,
                    "title": "節假日"
                }
                //{
                //    "id":    3,
                //    "title": "淡黃"
                //}
                ]
            };    
        }
    }
});