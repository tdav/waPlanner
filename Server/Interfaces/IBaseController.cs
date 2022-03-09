using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace waPlanner.Interfaces
{
    public interface IBaseController<T>
    {
        Task<ActionResult<IList<T>>> Get();
        Task<ActionResult<T>> Get(int id);
        Task<ActionResult<T>> Post([FromBody] T value);
        Task<ActionResult> Put([FromBody] T value);
        Task<ActionResult> Delete(int id);
    }
}
