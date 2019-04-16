using System.Collections.Generic;

namespace Beginor.AppFx.Core {

    public class PaginatedRequestModel {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }

    public class PaginatedResponseModel<T> : PaginatedRequestModel {
        public IList<T> Data { get; set; }
        public long Total { get; set; }
    }

}
