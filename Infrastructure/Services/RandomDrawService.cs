using Tournament.Application.Common.Interfaces;
using System;

namespace Tournament.Infrastructure.Services;

public class RandomDrawService:IDrawService
{
	private readonly Random _random;

	public RandomDrawService(){
		_random=new Random();
	}

	public  IEnumerable<(T item,int order)> Draw<T>(List<(T item,double weight)> ItemWeights,int ToDraw){
		var TotalWeight=ItemWeights.Sum(x=>x.weight);
		ItemWeights=ItemWeights.OrderBy(x=>x.weight).ToList();
		int Order=0;
		var DrawnList=new HashSet<int>();

		for (int j=0;j<ToDraw;j++){
			var rnd=_random.NextDouble()*TotalWeight;
			var sum=0d;
			for (int i=0;i<ItemWeights.Count();i++){
				if (DrawnList.Contains(i)){
					continue;
				}
				sum+=ItemWeights[i].weight;
				if (rnd<sum){

					TotalWeight-=ItemWeights[i].weight;
					DrawnList.Add(i);
					yield return (ItemWeights[i].item,++Order);
					break;

				}
			}
		}
	}
}
