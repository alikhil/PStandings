import * as React from 'react';
import * as moment from 'moment';
import * as DateRangePicker from 'react-bootstrap-daterangepicker'; 
import * as DateP from 'daterangepicker';


interface DateRangePickerEventHandler { (event?: any, picker?: any): any; }

interface DatePickerProps {
    startDate: moment.Moment;
    endDate: moment.Moment;
    onApply: DateRangePickerEventHandler;
    
}


export class DatePicker extends React.Component<DatePickerProps, {}> {
    
    public render() {
        return (
            <DateRangePicker 
                startDate={this.props.startDate} 
                endDate={this.props.endDate}
                ranges={{
                    'Сегодня': [moment(), moment()],
                    'Вчера': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Последние 7 дней': [moment().subtract(6, 'days'), moment()],
                    'Последние 30 дней': [moment().subtract(29, 'days'), moment()],
                    'Текущий месяц': [moment().startOf('month'), moment().endOf('month')],
                    'Прошлый месяц': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
                    'За все время': [moment('20160110','YYYYDDMM'), moment()]
                }}
               onApply={this.props.onApply}>
                {/* style={{width:'100%'}}  */}
                <button className="btn btn-default" type='button'> 
								<div className="pull-left mr-10"><span className="glyphicon glyphicon-calendar"> </span></div>
								<div className="pull-right">
									<span>
                                        {`${this.props.startDate.format('MMMM D, YYYY')} - ${this.props.endDate.format('MMMM D, YYYY')}`}
									</span>
									<span className="caret"></span>
								</div>
                </button>
            </DateRangePicker>
        );
    }

   
}
