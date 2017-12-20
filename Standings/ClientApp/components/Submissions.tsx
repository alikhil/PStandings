import { DatePicker } from './DateRangePicker';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import * as mm from 'moment';
import { Button } from 'react-bootstrap';
import autobind from 'autobind-decorator';

import { ComponentWithNav } from './ComponentWithNav';


const withQuery = require('with-query') as (url: string, ob: any) => string;

interface Submission {
    username: string;
    accepted: boolean;
    time: number;
    problemAlias: string;
    problemTitle: string;
    contest: string;
}

export interface DateRanges {
    startDate: Date;
    endDate: Date;
}

interface SubmissionsState extends DateRanges {
    submissions: Submission[];
    count: number;
    page: number;
    isLoading: boolean;
    prefix: string;
    
} 

export class SubmissionsComponent extends React.Component<RouteComponentProps<{ generation: string }>, SubmissionsState> {
    constructor() {
        super();
        this.state = { 
            submissions: [],
            isLoading: true,
            count: 200, 
            page: 0,
            prefix: "",
            startDate: new Date(2015, 1, 1),
            endDate: mm().toDate()
        };
    }

    public componentDidMount() {
        this.loadSumbmissions();
    }

    private loadSumbmissions() {
        this.setState({...this.state, isLoading: true });
        fetch(withQuery(`api/pcms/${this.props.match.params.generation}/submissions`, { 
                count: this.state.count,
                page: this.state.page,
                prefix: this.state.prefix,
                startDate: mm(this.state.startDate).unix(),
                endDate: mm(this.state.endDate).unix()
            }))
            .then(response => response.json() as Promise<Submission[]>)
            .then(data => this.setState({ ...this.state, isLoading: false, submissions: data }));
    }

    @autobind
    private updatePrefix(event: React.FormEvent<HTMLInputElement>) {
        this.setState({...this.state, prefix: event.currentTarget.value });
    }

    @autobind
    private onApply(event: string, picker: DateRanges) {
        this.setState({...this.state, startDate: mm(picker.startDate).toDate(), endDate: mm(picker.endDate).toDate() });
    }

    @autobind
    private onFilterClick() {
        this.setState({...this.state, page: 0 }, () => {
            this.loadSumbmissions();
        });
    }

    @autobind
    private onPrevPageClick() {
        this.setState({...this.state, page: this.state.page + 1}, () => {
            this.loadSumbmissions(); 
        });
    }

    @autobind
    private onNextPageClick() {
        this.setState({...this.state, page: this.state.page - 1}, () => {
            this.loadSumbmissions(); 
        });
    }

    public render() {
        const submissions = this.state.submissions.map((submission, i) =>
            <tr className={ submission.accepted ? 'success' : '' } key={ i }>
                <td className='ml-50'>{ submission.username }</td>
                <td className='tc'>{ mm.unix(submission.time).format("DD.MM.YYYY HH:mm") }</td>
                <td className='tc'>{ submission.problemAlias }</td>
                <td className='tc'>{ submission.problemTitle }</td>
                <td className='tc'>{ submission.contest }</td>
            </tr>
        );

        return <ComponentWithNav generation={this.props.match.params.generation}>
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
            <div  className='panel panel-default'>
                <table className='table'>
                    <thead>
                        <tr>
                            <td className='tc'>Участник</td>
                            <td className='tc'>Время</td>
                            <td className='tc'>Алиас</td>
                            <td className='tc'>Задача</td>
                            <td className='tc'>Контест</td>
                        </tr>
                    </thead>
                    <tbody>

                    { submissions }
                    </tbody>
                </table>
            </div>
                { this.state.isLoading ? <b>Loading...</b> : 
                <p className='clearfix text-center'>
                        {
                            this.state.submissions.length == this.state.count
                            ? <Button  type='button' onClick={ this.onPrevPageClick }><span aria-hidden='true'>&larr;</span> Старые посылки</Button>
                            : ''
                        }
                        { 
                            this.state.page > 0 
                            ? <Button type='button' onClick={this.onNextPageClick}>Новые посылки <span aria-hidden='true'>&rarr;</span></Button>
                            : ''
                        }
		        </p> }
        </ComponentWithNav>;
    }
}