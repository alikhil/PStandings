import autobind from 'autobind-decorator';
import * as mm from 'moment';
import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';

import { DatePicker } from '../DateRangePicker';
import { ComponentWithNav } from '../ComponentWithNav';
import { NavMenu } from '../NavMenu';
import { DateRanges } from '../Submissions';

interface CommonAnaliticsState extends DateRanges {
    isLoading: boolean;
    prefix: string;
}

export class CommonAnalyticsComponent extends React.Component<{}, CommonAnaliticsState> {
    
    constructor() {
        super();
        this.state = { 
            isLoading: true,
            prefix: "",
            startDate: new Date(2015, 1, 1),
            endDate: mm().toDate()
        };
    }

    @autobind
    private onApply(event: string, picker: DateRanges) {
        this.setState({...this.state, startDate: mm(picker.startDate).toDate(), endDate: mm(picker.endDate).toDate() });
    }

    @autobind
    private loadDataset() {

    }

    @autobind
    private onFilterClick() {
        this.loadDataset();
    }

    @autobind
    private updatePrefix(event: React.FormEvent<HTMLInputElement>) {
        this.setState({...this.state, prefix: event.currentTarget.value });
    }

    @autobind
    private handleSelect(key: any) {
        console.log(`selected ${key}`);
    }

    public render() {
        return <div> 
            
            <div>
                <form className='form-inline'>
                    <div className='form-group mr-10'>
                        <label htmlFor='groupInput' className='mr-10' >Группа</label>
                        <input type='text' className='form-control' id='groupInput' placeholder='гр1' onChange={this.updatePrefix}/>
                    </div>
                    <div className='form-group mr-10'>
                        <DatePicker 
                            startDate={mm(this.state.startDate)}
                            endDate={mm(this.state.endDate)} 
                            onApply={this.onApply}/>
                    </div>
                        <a className='btn btn-default' onClick={this.onFilterClick}> Отфильтровать</a>
                </form>
            </div>
            <br/>
        </div>;
    }
}